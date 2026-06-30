using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Data;
using MindCabinet.Data.DataAccess;
using MindCabinet.Services;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Utility.Attributes;
using System.Data;


namespace MindCabinet.Controllers;


// [HubRoute( ClientDataAccess_SimplePosts.IAPI.BaseRoute )]
// [Route("[controller]")]
[ApiController]
[Route( ClientDataAccess_SimplePosts.IAPI.BaseRoute )]
public class SimplePostController(
                ILogger<SimplePostController> logger,
                DbAccess dbAccess,
                ServerDataAccess_ServerData serverDataSrc,
                ServerDataAccess_UserAppData userAppDataSrc,
                ServerDataAccess_SimplePosts simplePostsDataSrc,
                ServerDataAccess_Terms termsDataSrc,
                ServerDataAccess_SimplePostTags simplePostTagsDataSrc,
                ServerDataAccess_UserTermsHistory userTermsHistoryDataSrc,
                ClientSessionManager sessMngr
            ) : ControllerBase, ClientDataAccess_SimplePosts.IAPI {
    private readonly ILogger<SimplePostController> Logger = logger;

    private readonly DbAccess DbAccess = dbAccess;

    private readonly ServerDataAccess_ServerData ServerDataSrc = serverDataSrc;

    private readonly ServerDataAccess_UserAppData UserAppDataSrc = userAppDataSrc;

    private readonly ServerDataAccess_SimplePosts SimplePostsDataSrc = simplePostsDataSrc;

    private readonly ServerDataAccess_Terms TermsDataSrc = termsDataSrc;

    private readonly ServerDataAccess_SimplePostTags SimplePostTagsDataSrc = simplePostTagsDataSrc;

    private readonly ServerDataAccess_UserTermsHistory UserTermsHistoryDataSrc = userTermsHistoryDataSrc;

    private readonly ClientSessionManager SessionManager = sessMngr;



    [HttpPost(nameof(GetByCriteriaForCurrentUser_Async))]
    public async Task<ClientDataAccess_SimplePosts.IAPI.GetByCriteria_Return> GetByCriteriaForCurrentUser_Async(
                ClientDataAccess_SimplePosts.IAPI.GetByCriteria_Params parameters ) {
        if( this.SessionManager.UserOfSession is null ) {
            throw new NullReferenceException( "Session not loaded." );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        IEnumerable<SimplePostObject.Raw> posts = await this.SimplePostsDataSrc.GetByCriteria_Async(
            dbCon: dbCon,
            termsDataSrc: this.TermsDataSrc,
            termSetsDataSrc: this.SimplePostTagsDataSrc,
            parameters: parameters,
            author: this.SessionManager.UserOfSession.Id
        );
        return new ClientDataAccess_SimplePosts.IAPI.GetByCriteria_Return { Posts = posts };
    }


    [HttpPost(nameof(GetCountByCriteriaForCurrentUser_Async))]
    public async Task<int> GetCountByCriteriaForCurrentUser_Async(
                ClientDataAccess_SimplePosts.IAPI.GetByCriteria_Params parameters ) {
        if( this.SessionManager.UserOfSession is null ) {
            throw new NullReferenceException( "Session not loaded." );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        return await this.SimplePostsDataSrc.GetCountByCriteria_Async(
            dbCon: dbCon,
            parameters: parameters,
            author: this.SessionManager.UserOfSession.Id
        );
    }


    [HttpPost(nameof(Create_Async))]
    public async Task<SimplePostObject.Raw> Create_Async(
                ClientDataAccess_SimplePosts.IAPI.Create_Params parameters ) {
        if( this.SessionManager.UserOfSession is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        return await this.SimplePostsDataSrc.Create_Async(
            dbCon: dbCon,
            serverData: this.ServerDataSrc,
            userData: this.UserAppDataSrc,
            termsData: this.TermsDataSrc,
            termSetsData: this.SimplePostTagsDataSrc,
            termHistoryData: this.UserTermsHistoryDataSrc,
            author: this.SessionManager.UserOfSession.Id,
            parameters: parameters,
            skipHistory: false
        );
    }
}
