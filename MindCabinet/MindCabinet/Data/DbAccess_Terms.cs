using Dapper;
using MindCabinet.Client.Services;
using MindCabinet.Shared.DataEntries;
using System;
using System.Data;


namespace MindCabinet.Data;


public partial class ServerDbAccess {
    public class TermEntryData {
        public long Id;
        public string Term = "";
        public long? ContextId;
        public long? AliasId;
        

        public async Task<TermEntry> Create_Async( IDbConnection dbCon, ServerDbAccess data ) {
            return new TermEntry(
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
                Id BIGINT NOT NULL IDENTITY(1, 1) PRIMARY KEY CLUSTERED,
                Term VARCHAR(64) NOT NULL,
                ContextId BIGINT,
                AliasId BIGINT,
                CONSTRAINT FK_ContextId FOREIGN KEY (ContextId)
                    REFERENCES Terms(Id),
                CONSTRAINT FK_AliasId FOREIGN KEY (AliasId)
                    REFERENCES Terms(Id)
            );"
        );

        return await this.InstallTermSets_Async( dbCon );
    }

    //



    private IDictionary<long, TermEntry> TermsById_Cache = new Dictionary<long, TermEntry>();



    public async Task<TermEntry?> GetTerm_Async( IDbConnection dbCon, long id ) {
        if( this.TermsById_Cache.ContainsKey(id) ) {
            return this.TermsById_Cache[id];
        }

        TermEntryData? termRaw = await dbCon.QuerySingleAsync<TermEntryData?>(
            "SELECT * FROM Terms AS MyTerms WHERE Id = @Id",
            new { Id = id }
        );

        if( termRaw is null ) {
            return null;
        }

        TermEntry term = await termRaw.Create_Async( dbCon, this );

        this.TermsById_Cache.Add( id, term );

        return term;
    }

    public async Task<IEnumerable<TermEntry>> GetTermsByCriteria_Async(
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

        //sql += @"ORDER BY Id
        //        OFFSET @Offset ROWS
        //        FETCH NEXT @Quantity ROWS ONLY;";
        //sqlParams["@Offset"] = parameters.Offset;
        //sqlParams["@Quantity"] = parameters.Quantity;

        IEnumerable<TermEntryData> terms = await dbCon.QueryAsync<TermEntryData>(
            sql, new DynamicParameters(sqlParams) );

        IList<TermEntry> termList = new List<TermEntry>( terms.Count() );

        foreach( TermEntryData term in terms ) {
            termList.Add( await term.Create_Async(dbCon, this) );
        }

        return termList;
	}


    public async Task<ClientDbAccess.CreateTermReturn> CreateTerm_Async(
                IDbConnection dbCon,
                ClientDbAccess.CreateTermParams parameters ) {
		IEnumerable<TermEntry> terms = await this.GetTermsByCriteria_Async(
            dbCon,
			new ClientDbAccess.GetTermsByCriteriaParams(
				termPattern: parameters.TermPattern,
				context: parameters.Context
			)
		);
		if( terms.Count() > 0 ) {
			return new ClientDbAccess.CreateTermReturn( false, terms.First() );
		}

        if( parameters.Context is not null && parameters.Context.Id is null ) {
            throw new DataException( "Context must be defined." );
        }
        if( parameters.Alias is not null && parameters.Alias.Id is null ) {
            throw new DataException( "Alias must be defined." );
        }

        long newId = await dbCon.QuerySingleAsync<long>(
            @"INSERT INTO Terms (Term, ContextId, AliasId)
                OUTPUT INSERTED.id
                VALUES (@Term, @ContextId, @AliasId)",
            //OUTPUT INSERTED.id",
            //SELECT SCOPE_IDENTITY()
            new {
                Term = parameters.TermPattern,
                ContextId = parameters.Context?.Id,
                AliasId = parameters.Alias?.Id,
            }
        );

        var newTerm = new TermEntry(
			id: newId,
			term: parameters.TermPattern,
			context: parameters.Context,
			alias: parameters.Alias
		);
		this.TermsById_Cache[newId] = newTerm;

        return new ClientDbAccess.CreateTermReturn( true, newTerm );
    }
}
