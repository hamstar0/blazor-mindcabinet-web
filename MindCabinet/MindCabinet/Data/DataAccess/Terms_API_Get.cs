using Dapper;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.Utility;
using MindCabinet.Utility;
using System;
using System.Data;
using System.Text;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_Terms : IServerDataAccess {
    public async Task<TermObject.Raw?> GetById_Async(
                IDbConnection dbCon,
                TermId id ) {
        if( id == 0 ) {
            throw new ArgumentException( "TermId is not valid (must be non-zero)." );
        }

        //

        if( ServerDataAccess_Terms.Cache_ById.TryGet(id, out var cached) ) {
            return cached;
        }

        //

        TermObject.Raw? termRaw = await dbCon.QuerySingleOrDefaultAsync<TermObject.Raw>(
            $"SELECT * FROM {TableName} AS MyTerms WHERE {TableColumn_Id} = @Id",
            new { Id = id }
        );

        //

        ServerDataAccess_Terms.Cache_ById.Set(
            key: id,
            value: termRaw,
            expiry: this.ServerSettings.CacheExpirationDuration
        );

        return termRaw;
    }

    public async Task<IEnumerable<TermObject.Raw>> GetByIds_Async(
                IDbConnection dbCon,
                IEnumerable<TermId> ids ) {
        if( ids.Any(id => id == 0) ) {
            throw new ArgumentException( "TermId is not valid (must be non-zero)." );
        }

        //

        var cachedTermsRaw = new List<TermObject.Raw>( ids.Count() );

        foreach( TermId id in ids ) {
            if( ServerDataAccess_Terms.Cache_ById.TryGet(id, out var cached) && cached is not null ) {
                cachedTermsRaw.Add( cached );
            }
        }

        ids = ids.Except( cachedTermsRaw.Select(t => t.Id) );
        if( !ids.Any() ) {
            return cachedTermsRaw;
        }

        //

        string sql = $@"SELECT * FROM {TableName} AS MyTerms
            WHERE MyTerms.{TableColumn_Id} IN @Ids";
        var sqlParams = new Dictionary<string, object> { { "@Ids", ids } };

        IEnumerable<TermObject.Raw> termsRaw = await dbCon.QueryAsync<TermObject.Raw>(
            sql, new DynamicParameters(sqlParams) );

        //

        foreach( TermObject.Raw termRaw in termsRaw ) {
            ServerDataAccess_Terms.Cache_ById.Set(
                key: termRaw.Id,
                value: termRaw,
                expiry: this.ServerSettings.CacheExpirationDuration
            );
        }

        //

        return termsRaw.Concat( cachedTermsRaw );
    }

    //

    public (string sql, Dictionary<string, object> sqlParams) GetTermsByCriteriaSQL(
                ClientDataAccess_Terms.IAPI.GetByCriteria_Params parameters,
                string allColumns ) {
        //TODO: SimpleSqlSelectBuilder
        string sql = $"SELECT {allColumns} FROM {TableName} AS MyTerms";
        var sqlParams = new Dictionary<string, object>();

        bool hasWhere = false;

        if( parameters.ContextTermId is not null || parameters.ContextTermPattern is not null ) {
            hasWhere = true;

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
        } else if( parameters.TermPattern is not null ) {
            hasWhere = true;
            
            sql += $" WHERE MyTerms.{TableColumn_Term} LIKE @Term";
            sqlParams["@Term"] = $"%{parameters.TermPattern}%";
        }

        if( parameters.AbbrevPattern is not null ) {
            if( hasWhere ) {
                sql += $"\n AND ";
            } else {
                sql += $"\n WHERE ";
            }
            sql += $"MyTerms.{TableColumn_Abbreviation} LIKE @Abbreviation";
            sqlParams["@Abbreviation"] = $"%{parameters.AbbrevPattern}%";
        }

        sql += $" ORDER BY Term {(parameters.SortAscendingByTerm ? "ASC" : "DESC")} ";

        return (sql, sqlParams);
    }

    
    public async Task<IEnumerable<TermObject.Raw>> GetTermsByCriteria_Async(
                IDbConnection dbCon,
                ClientDataAccess_Terms.IAPI.GetByCriteria_Params parameters ) {
        (string sql, Dictionary<string, object> sqlParams) = this.GetTermsByCriteriaSQL( parameters, "*" );

        //

//this.Logger.LogInformation( "Executing SQL: {Sql} with params {Params}", sql, sqlParams );
        IEnumerable<TermObject.Raw> termsRaw = await dbCon.QueryAsync<TermObject.Raw>(
            sql, new DynamicParameters(sqlParams) );
//this.Logger.LogInformation( "Retrieved {Count} terms", terms.Count() );

        foreach( TermObject.Raw termRaw in termsRaw ) {
            ServerDataAccess_Terms.Cache_ById.Set(
                key: termRaw.Id,
                value: termRaw,
                expiry: this.ServerSettings.CacheExpirationDuration
            );
        }

        //

        return termsRaw;
	}

	public async Task<int> GetTermsCountByCriteria_Async(
                IDbConnection dbCon,
                ClientDataAccess_Terms.IAPI.GetByCriteria_Params parameters ) {
        (string sql, Dictionary<string, object> sqlParams) = this.GetTermsByCriteriaSQL( parameters, "COUNT(*)" );

        return await dbCon.QuerySingleAsync<int>( sql, new DynamicParameters(sqlParams) );
	}
}
