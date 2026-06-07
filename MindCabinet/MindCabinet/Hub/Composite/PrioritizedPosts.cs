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

    private readonly ServerDataAccess_PrioritizedPosts PrioritizedPostsDataSrc;

    private readonly ServerDataAccess_Terms TermsDataSrc;

    private readonly ServerDataAccess_SimplePostTags PostTagsDataSrc;

    private readonly ServerDataAccess_UserTermsHistory UserTermsHistoryDataSrc;

    private readonly ServerDataAccess_PostsContexts PostsContextDataSrc;
    
    private readonly ServerDataAccess_PostsContextTermEntry PostsContextTermEntryDataSrc;

    private readonly ClientSessionManager SessionManager;



    public PrioritizedPostsController(
                DbAccess dbAccess,
                IServiceProvider serviceProvider,
                ServerDataAccess_PrioritizedPosts prioritizedPostsDataSrc,
                ServerDataAccess_Terms termsDataSrc,
                ServerDataAccess_SimplePostTags postTagsDataSrc,
                ServerDataAccess_PostsContexts postsContextDataSrc,
                ServerDataAccess_PostsContextTermEntry postsContextTermEntryDataSrc,
                ServerDataAccess_UserTermsHistory userTermsHistoryDataSrc,
                ClientSessionManager sessionManager ) {
        //this.HttpContext
        this.DbAccess = dbAccess;
        this.ServiceProvider = serviceProvider;
        this.PrioritizedPostsDataSrc = prioritizedPostsDataSrc;
        this.TermsDataSrc = termsDataSrc;
        this.PostTagsDataSrc = postTagsDataSrc;
        this.PostsContextDataSrc = postsContextDataSrc;
        this.PostsContextTermEntryDataSrc = postsContextTermEntryDataSrc;
        this.UserTermsHistoryDataSrc = userTermsHistoryDataSrc;
        this.SessionManager = sessionManager;
    }


    public async Task<IEnumerable<SimplePostObject.Raw>> GetByCriteriaForCurrentUser_Async(
                ClientDataAccess_PrioritizedPosts.IAPI.GetByCriteria_Params parameters ) {
        if( !this.SessionManager.IsLoaded ) {
            HttpContext? context = this.Context.GetHttpContext();
            if( context is null ) {
                throw new InvalidOperationException( $"No HttpContext in {this.GetType().Name}" );
            }
            await ClientSessionManager.LoadForHubRequest_Async( this.ServiceProvider );
        }

        if( this.SessionManager.UserAppDataOfSession?.UserDefaultTerm is null ) {
            //throw new Exception( "Session not loaded." );
            //this.Logger.LogInformation( "Session not loaded." );
            return [];
        }

        if( !parameters.AdditionalTagIds.Any(id => id == this.SessionManager.UserAppDataOfSession.UserDefaultTerm.Id) ) {
            parameters.AdditionalTagIds = parameters.AdditionalTagIds
                .Append( this.SessionManager.UserAppDataOfSession.UserDefaultTerm.Id )
                .ToArray();
        }
        
        
        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        return await this.PrioritizedPostsDataSrc.GetByCriteria_Async(
            dbCon: dbCon,
            postTagsDataSrc: this.PostTagsDataSrc,
            postsContextDataSrc: this.PostsContextDataSrc,
            postsContextTermEntryDataSrc: this.PostsContextTermEntryDataSrc,
            parameters: parameters
        );
    }

    public async Task<int> GetCountByCriteriaForCurrentUser_Async(
                ClientDataAccess_PrioritizedPosts.IAPI.GetByCriteria_Params parameters ) {
        if( !this.SessionManager.IsLoaded ) {
            HttpContext? context = this.Context.GetHttpContext();
            if( context is null ) {
                throw new InvalidOperationException( $"No HttpContext in {this.GetType().Name}" );
            }
            await ClientSessionManager.LoadForHubRequest_Async( this.ServiceProvider );
        }

        if( this.SessionManager.UserAppDataOfSession?.UserDefaultTerm is null ) {
            return 0;
        }
        
        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        return await this.PrioritizedPostsDataSrc.GetCountByCriteria_Async(
            dbCon: dbCon,
            postsContextDataSrc: this.PostsContextDataSrc,
            postsContextTermEntryDataSrc: this.PostsContextTermEntryDataSrc,
            parameters: parameters
        );
    }
}
