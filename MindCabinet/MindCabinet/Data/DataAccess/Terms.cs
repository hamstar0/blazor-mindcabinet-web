using Dapper;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects.Term;
using System;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_Terms : IServerDataAccess {
    public static async Task<TermObject> CreateTermObject_Async(
                IDbConnection dbCon,
                ServerDataAccess_Terms termsData,
                TermObject.DatabaseEntry entry ) {
        return new TermObject(
            id: entry.Id,
            term: entry.Term,
            context: entry.ContextTermId is not null
                ? await termsData.GetById_Async(dbCon, termsData, entry.ContextTermId.Value)
                : null,
            alias: entry.AliasTermId is not null
                ? await termsData.GetById_Async(dbCon, termsData, entry.AliasTermId.Value)
                : null
        );
    }

    //



    public const string TableName = "Terms";

	public async Task<bool> Install_Async( IDbConnection dbCon, ServerDataAccess_TermSets termsSetsData ) {
        // todo: fulltext index on 'Term'
        await dbCon.ExecuteAsync( $@"
            CREATE TABLE {TableName} (
                Id BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                Term VARCHAR(64) NOT NULL,
                ContextId BIGINT,
                AliasId BIGINT,
                CONSTRAINT FK_{TableName}_ContextId FOREIGN KEY (ContextId)
                    REFERENCES {TableName}(Id),
                CONSTRAINT FK_{TableName}_AliasId FOREIGN KEY (AliasId)
                    REFERENCES {TableName}(Id)
            );"
        );
        
        return await termsSetsData.Install_Async( dbCon );
    }

    //



    internal IDictionary<long, TermObject> TermsById_Cache = new Dictionary<long, TermObject>();



    public async Task<TermObject?> GetById_Async(
                IDbConnection dbCon,
                ServerDataAccess_Terms termsData,
                long id ) {
        if( this.TermsById_Cache.ContainsKey(id) ) {
            return this.TermsById_Cache[id];
        }

        TermObject.DatabaseEntry? termRaw = await dbCon.QuerySingleAsync<TermObject.DatabaseEntry?>(
            $"SELECT * FROM {TableName} AS MyTerms WHERE Id = @Id",
            new { Id = id }
        );

        if( termRaw is null ) {
            return null;
        }

        TermObject term = await ServerDataAccess_Terms.CreateTermObject_Async( dbCon, termsData, termRaw );

        this.TermsById_Cache.Add( id, term );

        return term;
    }

    public async Task<IEnumerable<TermObject>> GetByIds_Async(
                IDbConnection dbCon,
                ServerDataAccess_Terms termsData,
                IEnumerable<long> ids ) {
        if( ids.All(k => this.TermsById_Cache.ContainsKey(k)) ) {
            return ids.Select( id => this.TermsById_Cache[id] );
        }
        // todo: optimize query to fetch only remaining uncached terms

        string sql = $@"SELECT * FROM {TableName} AS MyTerms
            WHERE MyTerms.Id IN @Ids";
        var sqlParams = new Dictionary<string, object> { { "@Ids", ids } };

        IEnumerable<TermObject.DatabaseEntry> termsRaw = await dbCon.QueryAsync<TermObject.DatabaseEntry>(
            sql, new DynamicParameters(sqlParams) );

        var termList = new List<TermObject>( termsRaw.Count() );

        foreach( TermObject.DatabaseEntry termRaw in termsRaw ) {
            termList.Add( await ServerDataAccess_Terms.CreateTermObject_Async(dbCon, termsData, termRaw) );
        }

        return termList;
    }
    
    public async Task<IEnumerable<TermObject>> GetTermsByCriteria_Async(
                IDbConnection dbCon,
                ServerDataAccess_Terms termsData,
                ClientDataAccess_Terms.GetByCriteria_Params parameters ) {
        //var terms = this.Terms.Values
        //	.Where( t => t.DeepTest(parameters.TermPattern, parameters.Context) );

        string sql = $"SELECT * FROM {TableName} AS MyTerms";
        var sqlParams = new Dictionary<string, object>();

        if( parameters.Context is not null ) {
            if( parameters.Context.Id is null ) {
                sql += $@" INNER JOIN {TableName} AS CtxTerms
                    ON (MyTerms.Context.Id = CtxTerms.Id)
                    WHERE CtxTerms.Term = @ContextTerm";
                sqlParams["@ContextTerm"] = parameters.Context.Term!;
            } else {
                sql += $@" WHERE MyTerms.ContextId = @ContextId";
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
        IEnumerable<TermObject.DatabaseEntry> termsRaw = await dbCon.QueryAsync<TermObject.DatabaseEntry>(
            sql, new DynamicParameters(sqlParams) );
//this.Logger.LogInformation( "Retrieved {Count} terms", terms.Count() );

        var termList = new List<TermObject>( termsRaw.Count() );

        foreach( TermObject.DatabaseEntry termRaw in termsRaw ) {
            termList.Add( await ServerDataAccess_Terms.CreateTermObject_Async(dbCon, termsData, termRaw) );
        }

        return termList;
	}


    public async Task<ClientDataAccess_Terms.Create_Return> Create_Async(
                IDbConnection dbCon,
                ServerDataAccess_Terms termsData,
                ClientDataAccess_Terms.Create_Params parameters ) {
		IEnumerable<TermObject> terms = await this.GetTermsByCriteria_Async(
            dbCon: dbCon,
            termsData: termsData,
			parameters: new ClientDataAccess_Terms.GetByCriteria_Params(
				termPattern: parameters.TermPattern,
				context: parameters.Context?.ToPrototype() ?? null
			)
		);
		if( terms.Count() > 0 ) {
			return new ClientDataAccess_Terms.Create_Return( false, terms.First() );
		}

        long newId = await dbCon.ExecuteScalarAsync<long>(
            $@"INSERT INTO {TableName} (Term, ContextId, AliasId) 
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
