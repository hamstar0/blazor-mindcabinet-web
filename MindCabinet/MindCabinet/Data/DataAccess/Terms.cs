using Dapper;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using System;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_Terms : IServerDataAccess {
    public const string TableName = "Terms";

	public async Task<bool> Install_Async( IDbConnection dbCon ) {
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
        
        return true;
    }

    //



    internal IDictionary<long, TermObject.Raw> TermsById_Cache = new Dictionary<long, TermObject.Raw>();



    public async Task<TermObject.Raw?> GetById_Async(
                IDbConnection dbCon,
                long id ) {
        if( this.TermsById_Cache.ContainsKey(id) ) {
            return this.TermsById_Cache[id];
        }

        TermObject.Raw? termRaw = await dbCon.QuerySingleOrDefaultAsync<TermObject.Raw>(
            $"SELECT * FROM {TableName} AS MyTerms WHERE Id = @Id",
            new { Id = id }
        );

        if( termRaw is not null ) {
            this.TermsById_Cache.Add( id, termRaw );
        }

        return termRaw;
    }

    public async Task<IEnumerable<TermObject.Raw>> GetByIds_Async(
                IDbConnection dbCon,
                IEnumerable<long> ids ) {
        if( ids.All(k => this.TermsById_Cache.ContainsKey(k)) ) {
            return ids.Select( id => this.TermsById_Cache[id] );
        }
        // todo: optimize query to fetch only remaining uncached terms

        string sql = $@"SELECT * FROM {TableName} AS MyTerms
            WHERE MyTerms.Id IN @Ids";
        var sqlParams = new Dictionary<string, object> { { "@Ids", ids } };

        IEnumerable<TermObject.Raw> termsRaw = await dbCon.QueryAsync<TermObject.Raw>(
            sql, new DynamicParameters(sqlParams) );

        return termsRaw;
    }
    
    public async Task<IEnumerable<TermObject.Raw>> GetTermsByCriteria_Async(
                IDbConnection dbCon,
                ClientDataAccess_Terms.GetByCriteria_Params parameters ) {
        //var terms = this.Terms.Values
        //	.Where( t => t.DeepTest(parameters.TermPattern, parameters.Context) );

        string sql = $"SELECT * FROM {TableName} AS MyTerms";
        var sqlParams = new Dictionary<string, object>();

        if( parameters.ContextTermId is not null || parameters.ContextTermPattern is not null ) {
            if( parameters.ContextTermId is not null ) {
                sql += $@" WHERE MyTerms.ContextId = @ContextId";
                sqlParams["@ContextId"] = parameters.ContextTermId!;
            } else {
                sql += $@" INNER JOIN {TableName} AS CtxTerms
                    ON (MyTerms.Context.Id = CtxTerms.Id)
                    WHERE CtxTerms.Term = @ContextTerm";
                sqlParams["@ContextTerm"] = parameters.ContextTermPattern!;
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
        IEnumerable<TermObject.Raw> termsRaw = await dbCon.QueryAsync<TermObject.Raw>(
            sql, new DynamicParameters(sqlParams) );
//this.Logger.LogInformation( "Retrieved {Count} terms", terms.Count() );

        return termsRaw;
	}


    public async Task<ClientDataAccess_Terms.Create_Return> Create_Async(
                IDbConnection dbCon,
                ClientDataAccess_Terms.Create_Params parameters ) {
		IEnumerable<TermObject.Raw> matchingTerms = await this.GetTermsByCriteria_Async(
            dbCon: dbCon,
			parameters: new ClientDataAccess_Terms.GetByCriteria_Params(
				termPattern: parameters.TermPattern,
                contextTermId: parameters.Context?.Id,
                contextTermPattern: parameters.Context?.Term
			)
		);
		if( matchingTerms.Count() == 1 ) {
			return new ClientDataAccess_Terms.Create_Return( false, matchingTerms.First() );
		} else if( matchingTerms.Count() >= 2 ) {
            throw new Exception( "Multiple matching terms found." );
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

        var newTerm = new TermObject.Raw {
			Id = newId,
			Term = parameters.TermPattern,
			ContextTermId = parameters.Context?.Id,
			AliasTermId = parameters.Alias?.Id
		};
		this.TermsById_Cache[newId] = newTerm;

        return new ClientDataAccess_Terms.Create_Return( true, newTerm );
    }
}
