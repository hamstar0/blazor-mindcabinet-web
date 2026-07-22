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
        var sqlBuilder = new SimpleSqlSelectBuilder(
            $"{TableName} AS MyTerms",
            [allColumns]
        );
        var sqlParams = new Dictionary<string, object>();

        //

        if( parameters.ContextTermId is not null || !string.IsNullOrEmpty(parameters.ContextTermPattern) ) {
            if( parameters.ContextTermId is not null ) {
                sqlBuilder.AddWhereClause( $"MyTerms.ContextId = @ContextId" );
                sqlParams["@ContextId"] = parameters.ContextTermId!;
            } else {
                sqlBuilder.JoinClause = $@" INNER JOIN {TableName} AS CtxTerms
                    ON (MyTerms.Context.Id = CtxTerms.Id)
                    WHERE CtxTerms.Term = @ContextTerm";
                sqlParams["@ContextTerm"] = parameters.ContextTermPattern!;
            }
        }
        
        if( !string.IsNullOrEmpty(parameters.TermPattern) ) {
            sqlBuilder.AddWhereClause( $"MyTerms.{TableColumn_Term} LIKE @Term" );
            sqlParams["@Term"] = $"%{parameters.TermPattern}%";
        }

        if( parameters.AbbrevPattern is not null ) {
            sqlBuilder.AddWhereClause( $"MyTerms.{TableColumn_Abbreviation} LIKE @Abbreviation" );
            sqlParams["@Abbreviation"] = $"%{parameters.AbbrevPattern}%";
        }

        sqlBuilder.OrderByClause =  $"MyTerms.{TableColumn_Term} {(parameters.SortAscendingByTerm ? "ASC" : "DESC")}";

        return (sqlBuilder.Build(), sqlParams);
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
