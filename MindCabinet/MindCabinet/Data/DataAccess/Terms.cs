using Dapper;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects.Term;
using System;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_Terms {
    public class TermObjectDbData {
        public long Id = default;
        public string Term = "";
        public long? ContextId = null;
        public long? AliasId = null;
        

        public async Task<TermObject> Create_Async( IDbConnection dbCon, ServerDataAccess_Terms termData ) {
            return new TermObject(
                id: this.Id,
                term: this.Term,
                context: this.ContextId is not null ? await termData.GetTerm_Async(dbCon, this.ContextId.Value) : null,
                alias: this.AliasId is not null ? await termData.GetTerm_Async(dbCon, this.AliasId.Value) : null
            );
        }
    }

    //



	public async Task<bool> Install_Async( IDbConnection dbCon, ServerDataAccess_Terms_Sets termsSetsData ) {
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

        return await termsSetsData.Install_Async( dbCon );
    }

    //



    internal IDictionary<long, TermObject> TermsById_Cache = new Dictionary<long, TermObject>();



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
                ClientDataAccess_Terms.GetByCriteria_Params parameters ) {
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

        var termList = new List<TermObject>( terms.Count() );

        foreach( TermObjectDbData term in terms ) {
            termList.Add( await term.Create_Async(dbCon, this) );
        }

        return termList;
	}


    public async Task<ClientDataAccess_Terms.Create_Return> Create_Async(
                IDbConnection dbCon,
                ClientDataAccess_Terms.Create_Params parameters ) {
		IEnumerable<TermObject> terms = await this.GetTermsByCriteria_Async(
            dbCon,
			new ClientDataAccess_Terms.GetByCriteria_Params(
				termPattern: parameters.TermPattern,
				context: parameters.Context?.ToPrototype() ?? null
			)
		);
		if( terms.Count() > 0 ) {
			return new ClientDataAccess_Terms.Create_Return( false, terms.First() );
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

        return new ClientDataAccess_Terms.Create_Return( true, newTerm );
    }
}
