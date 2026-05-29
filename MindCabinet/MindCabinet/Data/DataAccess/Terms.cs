using Dapper;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using System;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_Terms : IServerDataAccess {
    public async Task<TermObject.Raw?> GetById_Async(
                IDbConnection dbCon,
                TermId id ) {
        if( id == 0 ) {
            throw new ArgumentException( "TermId is not valid (must be non-zero)." );
        }

        TermObject.Raw? termRaw = await dbCon.QuerySingleOrDefaultAsync<TermObject.Raw>(
            $"SELECT * FROM {TableName} AS MyTerms WHERE {TableColumn_Id} = @Id",
            new { Id = id }
        );

        return termRaw;
    }

    public async Task<IEnumerable<TermObject.Raw>> GetByIds_Async(
                IDbConnection dbCon,
                IEnumerable<TermId> ids ) {
        if( ids.Any(id => id == 0) ) {
            throw new ArgumentException( "TermId is not valid (must be non-zero)." );
        }

        string sql = $@"SELECT * FROM {TableName} AS MyTerms
            WHERE MyTerms.{TableColumn_Id} IN @Ids";
        var sqlParams = new Dictionary<string, object> { { "@Ids", ids } };

        IEnumerable<TermObject.Raw> termsRaw = await dbCon.QueryAsync<TermObject.Raw>(
            sql, new DynamicParameters(sqlParams) );

        return termsRaw;
    }
    
    public async Task<IEnumerable<TermObject.Raw>> GetTermsByCriteria_Async(
                IDbConnection dbCon,
                ClientDataAccess_Terms.IAPI.GetByCriteria_Params parameters ) {
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

            sql += $" AND MyTerms.{TableColumn_Term} LIKE @Term";
        } else {
            sql += $" WHERE MyTerms.{TableColumn_Term} LIKE @Term";
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


    public async Task<ClientDataAccess_Terms.IAPI.Create_Return> Create_Async(
                IDbConnection dbCon,
                ClientDataAccess_Terms.IAPI.Create_Params parameters ) {
        if( !TermObject.ValidateTerm(parameters.TermPattern) ) {
            throw new ArgumentException( "Term is not valid." );
        }

		IEnumerable<TermObject.Raw> matchingTerms = await this.GetTermsByCriteria_Async(
            dbCon: dbCon,
			parameters: new ClientDataAccess_Terms.IAPI.GetByCriteria_Params {
				TermPattern = parameters.TermPattern,
                ContextTermId = parameters.ContextId,
                ContextTermPattern = null
            }
		);
		if( matchingTerms.Count() == 1 ) {
			return new ClientDataAccess_Terms.IAPI.Create_Return { IsAdded = false, TermRaw = matchingTerms.First() };
		} else if( matchingTerms.Count() >= 2 ) {
            throw new Exception( "Multiple matching terms found." );
        }

        long newId = await dbCon.ExecuteScalarAsync<long>(
            $@"INSERT INTO {TableName} ({TableColumn_Term}, {TableColumn_ContextId}, {TableColumn_AliasId}) 
                VALUES (@Term, @ContextId, @AliasId);
            SELECT LAST_INSERT_ID();",
            //SELECT SCOPE_IDENTITY()
            new {
                Term = parameters.TermPattern,
                ContextId = parameters.ContextId,
                AliasId = parameters.AliasId,
            }
        );
        if( newId == 0 ) {
            throw new Exception( "Could not declare new term." );
        }

        var newTerm = TermObject.CreateRaw(
			id: (TermId)newId,
			term: parameters.TermPattern,
			contextId: parameters.ContextId,
			aliasId: parameters.AliasId
		);

        return new ClientDataAccess_Terms.IAPI.Create_Return { IsAdded = true, TermRaw = newTerm };
    }
}
