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
using MindCabinet.Shared.DataObjects.PostsQuery;
using MindCabinet.Utility.Attributes;
using System.Data;


namespace MindCabinet.Controllers.Composite;


// [HubRoute( ClientDataAccess_PrioritizedPosts.IAPI.BaseRoute )]
// [Route("[controller]")]
[ApiController]
[Route( ClientDataAccess_PrioritizedPosts.IAPI.BaseRoute )]
public class PrioritizedPostsController : ControllerBase, ClientDataAccess_PrioritizedPosts.IAPI {
    private readonly ILogger<PrioritizedPostsController> Logger;

    private readonly DbAccess DbAccess;

    private readonly IServiceProvider ServiceProvider;

    private readonly ServerDataAccess_PrioritizedPosts PrioritizedPostsDataSrc;

    private readonly ServerDataAccess_Terms TermsDataSrc;

    private readonly ServerDataAccess_SimplePostTags PostTagsDataSrc;

    private readonly ServerDataAccess_UserTermsHistory UserTermsHistoryDataSrc;

    private readonly ServerDataAccess_PostsQuery PostsQueryDataSrc;
    
    private readonly ServerDataAccess_PostsQueryTermEntry PostsQueryTermEntryDataSrc;

    private readonly ClientSessionManager SessionManager;



    public PrioritizedPostsController(
                ILogger<PrioritizedPostsController> logger,
                DbAccess dbAccess,
                IServiceProvider serviceProvider,
                ServerDataAccess_PrioritizedPosts prioritizedPostsDataSrc,
                ServerDataAccess_Terms termsDataSrc,
                ServerDataAccess_SimplePostTags postTagsDataSrc,
                ServerDataAccess_PostsQuery postsQueryDataSrc,
                ServerDataAccess_PostsQueryTermEntry postsQueryTermEntryDataSrc,
                ServerDataAccess_UserTermsHistory userTermsHistoryDataSrc,
                ClientSessionManager sessionManager ) {
        //this.HttpContext
        this.Logger = logger;
        this.DbAccess = dbAccess;
        this.ServiceProvider = serviceProvider;
        this.PrioritizedPostsDataSrc = prioritizedPostsDataSrc;
        this.TermsDataSrc = termsDataSrc;
        this.PostTagsDataSrc = postTagsDataSrc;
        this.PostsQueryDataSrc = postsQueryDataSrc;
        this.PostsQueryTermEntryDataSrc = postsQueryTermEntryDataSrc;
        this.UserTermsHistoryDataSrc = userTermsHistoryDataSrc;
        this.SessionManager = sessionManager;
    }


    [HttpPost(nameof(GetByCriteriaForCurrentUser_Async))]
    public async Task<IEnumerable<SimplePostObject.Raw>> GetByCriteriaForCurrentUser_Async(
                ClientDataAccess_PrioritizedPosts.IAPI.GetByCriteria_Params parameters ) {
        if( this.SessionManager.UserAppDataOfSession?.UserDefaultTerm is null ) {
            //throw new Exception( "Session not loaded." );
            //this.Logger.LogInformation( "Session not loaded." );
            return [];
        }

        // SimpleUserId currUserId = this.SessionManager.UserAppDataOfSession.SimpleUserId;
        
        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );
        
        PostsQueryObject.Raw? raw = await this.PostsQueryDataSrc.GetById_Async(
            dbCon: dbCon,
            postsQueryTermEntryDataSrc: this.PostsQueryTermEntryDataSrc,
            postsQueryId: parameters.PostsQueryId,
            alsoGetEntries: true
        );
        if( raw is null ) {
            this.Logger.LogWarning( "Missing PostsQueryObject raw?" );
            return [];
        }

        if( raw.Owner != this.SessionManager.UserOfSession?.Id ) {
            this.Logger.LogWarning( "watch this leet haxor boi!" );
            return [];
        }

        return await this.PrioritizedPostsDataSrc.GetByCriteria_Async(
            dbCon: dbCon,
            postTagsDataSrc: this.PostTagsDataSrc,
            postsQuery: raw,
            parameters: parameters
        );
    }


    [HttpPost(nameof(GetCountByCriteriaForCurrentUser_Async))]
    public async Task<int> GetCountByCriteriaForCurrentUser_Async(
                ClientDataAccess_PrioritizedPosts.IAPI.GetByCriteria_Params parameters ) {
        if( this.SessionManager.UserAppDataOfSession?.UserDefaultTerm is null ) {
            return 0;
        }
        
        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        return await this.PrioritizedPostsDataSrc.GetCountByCriteria_Async(
            dbCon: dbCon,
            postsQueryDataSrc: this.PostsQueryDataSrc,
            postsQueryTermEntryDataSrc: this.PostsQueryTermEntryDataSrc,
            parameters: parameters
        );
    }
}
