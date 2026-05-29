using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Client.Services.DbAccess.Joined;
using MindCabinet.Data;
using MindCabinet.Data.DataAccess;
using MindCabinet.Data.DataAccess.Composite;
using MindCabinet.Services;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Utility.Attributes;
using System.Data;
using static MindCabinet.Data.DataAccess.Composite.ServerDataAccess_PrioritizedPosts;


namespace MindCabinet.Hubs.Composite;


[HubRoute( ClientDataAccess_PrioritizedPosts.IAPI.BaseRoute )]
public class PrioritizedPostsController : Hub, ClientDataAccess_PrioritizedPosts.IAPI {
    private readonly DbAccess DbAccess;

    private readonly ServerDataAccess_PrioritizedPosts PrioritizedPostsData;

    private readonly ServerDataAccess_Terms TermsData;

    private readonly ServerDataAccess_SimplePostTags PostTagsData;

    private readonly ServerDataAccess_UserTermsHistory UserTermsHistoryData;

    private readonly ServerDataAccess_PostsContexts PostsContextData;
    
    private readonly ServerDataAccess_PostsContextTermEntry PostsContextTermEntryData;



    public PrioritizedPostsController(
                DbAccess dbAccess,
                ServerDataAccess_PrioritizedPosts prioritizedPostsData,
                ServerDataAccess_Terms termsData,
                ServerDataAccess_SimplePostTags postTagsData,
                ServerDataAccess_PostsContexts postsContextData,
                ServerDataAccess_PostsContextTermEntry postsContextTermEntryData,
                ServerDataAccess_UserTermsHistory userTermsHistoryData ) {
        //this.HttpContext
        this.DbAccess = dbAccess;
        this.PrioritizedPostsData = prioritizedPostsData;
        this.TermsData = termsData;
        this.PostTagsData = postTagsData;
        this.PostsContextData = postsContextData;
        this.PostsContextTermEntryData = postsContextTermEntryData;
        this.UserTermsHistoryData = userTermsHistoryData;
    }


    public async Task<IEnumerable<SimplePostObject.Raw>> GetByCriteria_Async(
                ClientDataAccess_PrioritizedPosts.IAPI.GetByCriteria_Params parameters ) {
        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        return await this.PrioritizedPostsData.GetByCriteria_Async(
            dbCon: dbCon,
            termsData: this.TermsData,
            postTagsData: this.PostTagsData,
            postsContextData: this.PostsContextData,
            postsContextTermEntryData: this.PostsContextTermEntryData,
            parameters: parameters
        );
    }

    public async Task<int> GetCountByCriteria_Async(
                ClientDataAccess_PrioritizedPosts.IAPI.GetByCriteria_Params parameters ) {
        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        return await this.PrioritizedPostsData.GetCountByCriteria_Async(
            dbCon: dbCon,
            postsContextData: this.PostsContextData,
            postsContextTermEntryData: this.PostsContextTermEntryData,
            parameters: parameters
        );
    }
}
