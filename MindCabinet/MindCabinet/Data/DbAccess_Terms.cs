using Dapper;
using MindCabinet.Client.Services;
using MindCabinet.Shared.DataObjects.Term;
using System;
using System.Data;


namespace MindCabinet.Data;


public partial class ServerDbAccess {
    private class TermObjectDbData {
        public long Id = default;
        public string Term = "";
        public long? ContextId = null;
        public long? AliasId = null;
        

        public async Task<TermObject> Create_Async( IDbConnection dbCon, ServerDbAccess data ) {
            return new TermObject(
                id: this.Id,
                term: this.Term,
                context: this.ContextId is not null ? await data.GetTerm_Async(dbCon, this.ContextId.Value) : null,
                alias: this.AliasId is not null ? await data.GetTerm_Async(dbCon, this.AliasId.Value) : null
            );
        }
    }

    //



	public async Task<bool> InstallTerms_Async( IDbConnection dbCon ) {
        // todo: fulltext index on 'Term'
        await dbCon.ExecuteAsync( @"
            CREATE TABLE Terms (
                Id BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                Term VARCHAR(64) NOT NULL,
                ContextId BIGINT,
                AliasId BIGINT,
                CONSTRAINT FK_ContextTermId FOREIGN KEY (ContextId)
                    REFERENCES Terms(Id),
                CONSTRAINT FK_AliasTermId FOREIGN KEY (AliasId)
                    REFERENCES Terms(Id)
            );"
        );

        return await this.InstallTermSets_Async( dbCon );
    }

    //



    private IDictionary<long, TermObject> TermsById_Cache = new Dictionary<long, TermObject>();



    public async Task<TermObject?> GetTerm_Async( IDbConnection dbCon, long id ) {
        if( this.TermsById_Cache.ContainsKey(id) ) {
            return this.TermsById_Cache[id];
        }

        TermObjectDbData? termRaw = await dbCon.QuerySingleAsync<TermObjectDbData?>(
            "SELECT * FROM Terms AS MyTerms WHERE Id = @Id",
            new { Id = id }
        );

        if( termRaw is null ) {
            return null;
        }

        TermObject term = await termRaw.Create_Async( dbCon, this );

        this.TermsById_Cache.Add( id, term );

        return term;
    }

    public async Task<IEnumerable<TermObject>> GetTermsByCriteria_Async(
                IDbConnection dbCon,
                ClientDbAccess.GetTermsByCriteriaParams parameters ) {
        //var terms = this.Terms.Values
        //	.Where( t => t.DeepTest(parameters.TermPattern, parameters.Context) );

        string sql = @"SELECT * FROM Terms AS MyTerms";
        var sqlParams = new Dictionary<string, object>();

        if( parameters.Context is not null ) {
            if( parameters.Context.Id is null ) {
                sql += @" INNER JOIN Terms AS CtxTerms
                    ON (MyTerms.Context.Id = CtxTerms.Id)
                    WHERE CtxTerms.Term = @ContextTerm";
                sqlParams["@ContextTerm"] = parameters.Context.Term!;
            } else {
                sql += @" WHERE MyTerms.ContextId = @ContextId";
                sqlParams["@ContextId"] = parameters.Context.Id!;
            }

            sql += " AND MyTerms.Term LIKE @Term";
        } else {
            sql += " WHERE MyTerms.Term LIKE @Term";
        }
        sqlParams["@Term"] = $"%{parameters.TermPattern}%";

        //sql += @"ORDER BY Id      <- outdated SQL Server sql!
        //        OFFSET @Offset ROWS
        //        FETCH NEXT @Quantity ROWS ONLY;";
        //sqlParams["@Offset"] = parameters.Offset;
        //sqlParams["@Quantity"] = parameters.Quantity;

//this.Logger.LogInformation( "Executing SQL: {Sql} with params {Params}", sql, sqlParams );
        IEnumerable<TermObjectDbData> terms = await dbCon.QueryAsync<TermObjectDbData>(
            sql, new DynamicParameters(sqlParams) );
//this.Logger.LogInformation( "Retrieved {Count} terms", terms.Count() );

        IList<TermObject> termList = new List<TermObject>( terms.Count() );

        foreach( TermObjectDbData term in terms ) {
            termList.Add( await term.Create_Async(dbCon, this) );
        }

        return termList;
	}


    public async Task<ClientDbAccess.CreateTermReturn> CreateTerm_Async(
                IDbConnection dbCon,
                ClientDbAccess.CreateTermParams parameters ) {
		IEnumerable<TermObject> terms = await this.GetTermsByCriteria_Async(
            dbCon,
			new ClientDbAccess.GetTermsByCriteriaParams(
				termPattern: parameters.TermPattern,
				context: parameters.Context?.ToPrototype() ?? null
			)
		);
		if( terms.Count() > 0 ) {
			return new ClientDbAccess.CreateTermReturn( false, terms.First() );
		}

        long newId = await dbCon.ExecuteScalarAsync<long>(
            @"INSERT INTO Terms (Term, ContextId, AliasId) 
                VALUES (@Term, @ContextId, @AliasId);
            SELECT LAST_INSERT_ID();",
            //SELECT SCOPE_IDENTITY()
            new {
                Term = parameters.TermPattern,
                ContextId = parameters.Context?.Id,
                AliasId = parameters.Alias?.Id,
            }
        );

        var newTerm = new TermObject(
			id: newId,
			term: parameters.TermPattern,
			context: parameters.Context,
			alias: parameters.Alias
		);
		this.TermsById_Cache[newId] = newTerm;

        return new ClientDbAccess.CreateTermReturn( true, newTerm );
    }
}
