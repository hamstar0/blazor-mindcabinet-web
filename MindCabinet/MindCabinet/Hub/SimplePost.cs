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


namespace MindCabinet.Hubs;


[HubRoute( ClientDataAccess_SimplePosts.IAPI.BaseRoute )]
public class SimplePostController(
                ILogger<SimplePostController> logger,
                IServiceProvider serviceProvider,
                DbAccess dbAccess,
                ServerDataAccess_ServerData serverDataSrc,
                ServerDataAccess_UserAppData userAppDataSrc,
                ServerDataAccess_SimplePosts simplePostsDataSrc,
                ServerDataAccess_Terms termsDataSrc,
                ServerDataAccess_SimplePostTags simplePostTagsDataSrc,
                ServerDataAccess_UserTermsHistory userTermsHistoryDataSrc,
                ClientSessionManager sessMngr
            ) : Hub, ClientDataAccess_SimplePosts.IAPI {
    private readonly ILogger<SimplePostController> Logger = logger;

    private readonly IServiceProvider ServiceProvider = serviceProvider;

    private readonly DbAccess DbAccess = dbAccess;

    private readonly ServerDataAccess_ServerData ServerDataSrc = serverDataSrc;

    private readonly ServerDataAccess_UserAppData UserAppDataSrc = userAppDataSrc;

    private readonly ServerDataAccess_SimplePosts SimplePostsDataSrc = simplePostsDataSrc;

    private readonly ServerDataAccess_Terms TermsDataSrc = termsDataSrc;

    private readonly ServerDataAccess_SimplePostTags SimplePostTagsDataSrc = simplePostTagsDataSrc;

    private readonly ServerDataAccess_UserTermsHistory UserTermsHistoryDataSrc = userTermsHistoryDataSrc;

    private readonly ClientSessionManager SessionManager = sessMngr;



    public async Task<ClientDataAccess_SimplePosts.IAPI.GetByCriteria_Return> GetByCriteria_Async(
                ClientDataAccess_SimplePosts.IAPI.GetByCriteria_Params parameters ) {
        if( !this.SessionManager.IsLoaded ) {
            HttpContext? context = this.Context.GetHttpContext();
            if( context is null ) {
                throw new InvalidOperationException( $"No HttpContext in {this.GetType().Name}" );
            }
            await ClientSessionManager.LoadForHubRequest_Async( this.ServiceProvider );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        IEnumerable<SimplePostObject.Raw> posts = await this.SimplePostsDataSrc.GetByCriteria_Async(
            dbCon,
            this.TermsDataSrc,
            this.SimplePostTagsDataSrc,
            parameters
        );
        return new ClientDataAccess_SimplePosts.IAPI.GetByCriteria_Return { Posts = posts };
    }

    public async Task<int> GetCountByCriteria_Async(
                ClientDataAccess_SimplePosts.IAPI.GetByCriteria_Params parameters ) {
        if( !this.SessionManager.IsLoaded ) {
            HttpContext? context = this.Context.GetHttpContext();
            if( context is null ) {
                throw new InvalidOperationException( $"No HttpContext in {this.GetType().Name}" );
            }
            await ClientSessionManager.LoadForHubRequest_Async( this.ServiceProvider );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        return await this.SimplePostsDataSrc.GetCountByCriteria_Async( dbCon, parameters );
    }


    public async Task<SimplePostObject.Raw> Create_Async(
                ClientDataAccess_SimplePosts.IAPI.Create_Params parameters ) {
        if( !this.SessionManager.IsLoaded ) {
            HttpContext? context = this.Context.GetHttpContext();
            if( context is null ) {
                throw new InvalidOperationException( $"No HttpContext in {this.GetType().Name}" );
            }
            await ClientSessionManager.LoadForHubRequest_Async( this.ServiceProvider );
        }

        if( this.SessionManager.UserOfSession is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        SimplePostId simplePostId = await this.SimplePostsDataSrc.Create_Async(
            dbCon: dbCon,
            serverData: this.ServerDataSrc,
            userData: this.UserAppDataSrc,
            termsData: this.TermsDataSrc,
            termSetsData: this.SimplePostTagsDataSrc,
            termHistoryData: this.UserTermsHistoryDataSrc,
            simpleUserId: this.SessionManager.UserOfSession.Id,
            parameters: parameters,
            addCurrentUserTag: true,
            skipHistory: false
        );
        return SimplePostObject.CreateRaw(
            id: simplePostId,
            //SimpleUserId = this.SessionData.UserOfSession.Id,
            created: DateTime.UtcNow,
            modified: DateTime.UtcNow,
            simpleUserId: this.SessionManager.UserOfSession.Id,
            body: parameters.Body,
            tagsTermIdSet: parameters.TermIds.ToArray()
        );
    }
}
