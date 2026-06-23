using Dapper;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Client.Services.DbAccess.Joined;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.PostsContext;
using System.Data;
using System.Text.Json;
using static MindCabinet.Data.DataAccess.ServerDataAccess_SimplePosts;


namespace MindCabinet.Data.DataAccess.Composite;


public partial class ServerDataAccess_PrioritizedPosts(
                ILogger<ServerDataAccess_PrioritizedPosts> logger
            ) : IServerDataAccess {
    private readonly ILogger<ServerDataAccess_PrioritizedPosts> Logger = logger;
    

    
    public async Task<SimplePostObject.Raw[]> GetByCriteria_Async(
                IDbConnection dbCon,
                ServerDataAccess_SimplePostTags postTagsDataSrc,
                PostsContextObject.Raw postsContext,
                ClientDataAccess_PrioritizedPosts.IAPI.GetByCriteria_Params parameters ) {
        if( parameters.PostsPerPage == 0 ) {
            return [];
        }

        (string sql, IDictionary<string, object> sqlParams) = this.GetByCriteriaSql(
            postsContext: postsContext,
            bodyPattern: parameters.BodyPattern,
            sortAscendingByDate: parameters.SortAscendingByDate,
            postsPerPage: parameters.PostsPerPage,
            pageNumber: parameters.PageNumber,
            additionalRequiredTagIds: parameters.AdditionalTagIds,
            countOnly: false
        );

// this.Logger.LogInformation( "Executing SQL: {Sql} with params {Params}", sql, JsonSerializer.Serialize(sqlParams) );
        IEnumerable<SimplePostObject.Raw> posts = await dbCon.QueryAsync<SimplePostObject.Raw>(
            sql, new DynamicParameters( sqlParams )
        );
        
        foreach( SimplePostObject.Raw post in posts ) {
            post.TagsTermIdSet = (await postTagsDataSrc.Get_Async( dbCon, post.Id ))
                .Select( t => t.Id )
                .ToArray();
        }

// this.Logger.LogInformation( "Result: {posts}", JsonSerializer.Serialize(posts) );
        return posts.ToArray();
	}


    public async Task<int> GetCountByCriteria_Async(
                IDbConnection dbCon,
                ServerDataAccess_PostsContexts postsContextDataSrc,
                ServerDataAccess_PostsContextTermEntry postsContextTermEntryDataSrc,
                ClientDataAccess_PrioritizedPosts.IAPI.GetByCriteria_Params parameters ) {
        if( parameters.PostsPerPage == 0 ) {
            return 0;
        }

        PostsContextObject.Raw? usrCtx = await postsContextDataSrc.GetById_Async(
            dbCon: dbCon,
            postsContextTermEntryDataSrc: postsContextTermEntryDataSrc,
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
                additionalRequiredTagIds: parameters.AdditionalTagIds,
                countOnly: true
        );

        return await dbCon.QuerySingleAsync<int>( sql, new DynamicParameters(sqlParams) );
    }
}
