using Dapper;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Client.Services.DbAccess.Joined;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.UserContext;
using System.Data;
using static MindCabinet.Data.DataAccess.ServerDataAccess_SimplePosts;


namespace MindCabinet.Data.DataAccess.Composite;


public partial class ServerDataAccess_PrioritizedPosts : IServerDataAccess {
    public string TableName => ServerDataAccess_SimplePosts.TableName;
    

    
    public async Task<IEnumerable<SimplePostObject.DatabaseEntry>> GetByCriteria_Async(
                IDbConnection dbCon,
                ServerDataAccess_UserContexts userContextData,
                ClientDataAccess_PrioritizedPosts.GetByCriteria_Params parameters ) {
        if( parameters.PostsPerPage == 0 ) {
            return [];
        }

        UserContextObject.DatabaseEntry? usrCtx = await userContextData.GetById_Async( dbCon, parameters.UserContextId );
        if( usrCtx is null ) {
            return [];
        }

        (string sql, IDictionary<string, object> sqlParams) = this.GetByCriteriaSql(
                userContext: usrCtx,
                bodyPattern: parameters.BodyPattern,
                sortAscendingByDate: parameters.SortAscendingByDate,
                postsPerPage: parameters.PostsPerPage,
                pageNumber: parameters.PageNumber,
                additionalTagIds: parameters.AdditionalTagIds,
                countOnly: false
        );

        // this.Logger.LogInformation( "Executing SQL: {Sql} with params {Params}", sql, sqlParams );
        IEnumerable<SimplePostObject.DatabaseEntry> posts = await dbCon.QueryAsync<SimplePostObject.DatabaseEntry>(
            sql, new DynamicParameters( sqlParams )
        );

        return posts;
	}


    public async Task<int> GetCountByCriteria_Async(
                IDbConnection dbCon,
                ServerDataAccess_UserContexts userContextData,
                ClientDataAccess_PrioritizedPosts.GetByCriteria_Params parameters ) {
        if( parameters.PostsPerPage == 0 ) {
            return 0;
        }

        UserContextObject.DatabaseEntry? usrCtx = await userContextData.GetById_Async( dbCon, parameters.UserContextId );
        if( usrCtx is null ) {
            return 0;
        }

        (string sql, IDictionary<string, object> sqlParams) = this.GetByCriteriaSql(
                userContext: usrCtx,
                bodyPattern: parameters.BodyPattern,
                sortAscendingByDate: parameters.SortAscendingByDate,
                postsPerPage: parameters.PostsPerPage,
                pageNumber: parameters.PageNumber,
                additionalTagIds: parameters.AdditionalTagIds,
                countOnly: true
        );

        return await dbCon.QuerySingleAsync<int>( sql, new DynamicParameters(sqlParams) );
    }
}
