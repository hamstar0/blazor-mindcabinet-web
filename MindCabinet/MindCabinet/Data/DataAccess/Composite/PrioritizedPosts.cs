using Dapper;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Client.Services.DbAccess.Joined;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.PostsContext;
using System.Data;
using static MindCabinet.Data.DataAccess.ServerDataAccess_SimplePosts;


namespace MindCabinet.Data.DataAccess.Composite;


public partial class ServerDataAccess_PrioritizedPosts( ILogger<ServerDataAccess_PrioritizedPosts> logger ) : IServerDataAccess {
    private readonly ILogger<ServerDataAccess_PrioritizedPosts> Logger = logger;
    

    
    public async Task<SimplePostObject.Raw[]> GetByCriteria_Async(
                IDbConnection dbCon,
                ServerDataAccess_PostsContexts postsContextData,
                ServerDataAccess_PostsContextTermEntry postsContextTermEntryData,
                ClientDataAccess_PrioritizedPosts.GetByCriteria_Params parameters ) {
        if( parameters.PostsPerPage == 0 ) {
            return [];
        }

        PostsContextObject.Raw? usrCtx = await postsContextData.GetById_Async(
            dbCon: dbCon,
            postsContextTermEntryData: postsContextTermEntryData,
            postsContextId: parameters.PostsContextId,
            alsoGetEntries: true
        );
        if( usrCtx is null ) {
            return [];
        }

        (string sql, IDictionary<string, object> sqlParams) = this.GetByCriteriaSql(
                postsContext: usrCtx,
                bodyPattern: parameters.BodyPattern,
                sortAscendingByDate: parameters.SortAscendingByDate,
                postsPerPage: parameters.PostsPerPage,
                pageNumber: parameters.PageNumber,
                additionalTagIds: parameters.AdditionalTagIds,
                countOnly: false
        );

        // this.Logger.LogInformation( "Executing SQL: {Sql} with params {Params}", sql, sqlParams );
        IEnumerable<SimplePostObject.Raw> posts = await dbCon.QueryAsync<SimplePostObject.Raw>(
            sql, new DynamicParameters( sqlParams )
        );

        return posts.ToArray();
	}


    public async Task<int> GetCountByCriteria_Async(
                IDbConnection dbCon,
                ServerDataAccess_PostsContexts postsContextData,
                ServerDataAccess_PostsContextTermEntry postsContextTermEntryData,
                ClientDataAccess_PrioritizedPosts.GetByCriteria_Params parameters ) {
        if( parameters.PostsPerPage == 0 ) {
            return 0;
        }

        PostsContextObject.Raw? usrCtx = await postsContextData.GetById_Async(
            dbCon: dbCon,
            postsContextTermEntryData: postsContextTermEntryData,
            postsContextId: parameters.PostsContextId,
            alsoGetEntries: true
        );
        if( usrCtx is null ) {
            return 0;
        }

        (string sql, IDictionary<string, object> sqlParams) = this.GetByCriteriaSql(
                postsContext: usrCtx,
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
