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


namespace MindCabinet.Hubs.Composite;


[HubRoute( ClientDataAccess_PrioritizedPosts.IAPI.BaseRoute )]
public class PrioritizedPostsController : Hub, ClientDataAccess_PrioritizedPosts.IAPI {
    private readonly DbAccess DbAccess;

    private readonly IServiceProvider ServiceProvider;

    private readonly ServerDataAccess_PrioritizedPosts PrioritizedPostsData;

    private readonly ServerDataAccess_Terms TermsData;

    private readonly ServerDataAccess_SimplePostTags PostTagsData;

    private readonly ServerDataAccess_UserTermsHistory UserTermsHistoryData;

    private readonly ServerDataAccess_PostsContexts PostsContextData;
    
    private readonly ServerDataAccess_PostsContextTermEntry PostsContextTermEntryData;

    private readonly ClientSessionManager SessionManager;



    public PrioritizedPostsController(
                DbAccess dbAccess,
                IServiceProvider serviceProvider,
                ServerDataAccess_PrioritizedPosts prioritizedPostsData,
                ServerDataAccess_Terms termsData,
                ServerDataAccess_SimplePostTags postTagsData,
                ServerDataAccess_PostsContexts postsContextData,
                ServerDataAccess_PostsContextTermEntry postsContextTermEntryData,
                ServerDataAccess_UserTermsHistory userTermsHistoryData,
                ClientSessionManager sessionManager ) {
        //this.HttpContext
        this.DbAccess = dbAccess;
        this.ServiceProvider = serviceProvider;
        this.PrioritizedPostsData = prioritizedPostsData;
        this.TermsData = termsData;
        this.PostTagsData = postTagsData;
        this.PostsContextData = postsContextData;
        this.PostsContextTermEntryData = postsContextTermEntryData;
        this.UserTermsHistoryData = userTermsHistoryData;
        this.SessionManager = sessionManager;
    }


    public async Task<IEnumerable<SimplePostObject.Raw>> GetByCriteria_Async(
                ClientDataAccess_PrioritizedPosts.IAPI.GetByCriteria_Params parameters ) {
        if( !this.SessionManager.IsLoaded ) {
            HttpContext? context = this.Context.GetHttpContext();
            if( context is null ) {
                throw new InvalidOperationException( $"No HttpContext in {this.GetType().Name}" );
            }
            await ClientSessionManager.LoadForHubRequest_Async( this.ServiceProvider );
        }

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
        if( !this.SessionManager.IsLoaded ) {
            HttpContext? context = this.Context.GetHttpContext();
            if( context is null ) {
                throw new InvalidOperationException( $"No HttpContext in {this.GetType().Name}" );
            }
            await ClientSessionManager.LoadForHubRequest_Async( this.ServiceProvider );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        return await this.PrioritizedPostsData.GetCountByCriteria_Async(
            dbCon: dbCon,
            postsContextData: this.PostsContextData,
            postsContextTermEntryData: this.PostsContextTermEntryData,
            parameters: parameters
        );
    }
}
