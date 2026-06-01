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
                ServerDataAccess_ServerData serverData,
                ServerDataAccess_UserAppData userAppData,
                ServerDataAccess_SimplePosts simplePostsData,
                ServerDataAccess_Terms termsData,
                ServerDataAccess_SimplePostTags simplePostTagsData,
                ServerDataAccess_UserTermsHistory userTermsHistoryData,
                ClientSessionManager sessMngr
            ) : Hub, ClientDataAccess_SimplePosts.IAPI {
    private readonly ILogger<SimplePostController> Logger = logger;

    private readonly IServiceProvider ServiceProvider = serviceProvider;

    private readonly DbAccess DbAccess = dbAccess;

    private readonly ServerDataAccess_ServerData ServerData = serverData;

    private readonly ServerDataAccess_UserAppData UserAppData = userAppData;

    private readonly ServerDataAccess_SimplePosts SimplePostsData = simplePostsData;

    private readonly ServerDataAccess_Terms TermsData = termsData;

    private readonly ServerDataAccess_SimplePostTags SimplePostTagsData = simplePostTagsData;

    private readonly ServerDataAccess_UserTermsHistory UserTermsHistoryData = userTermsHistoryData;

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

        IEnumerable<SimplePostObject.Raw> posts = await this.SimplePostsData.GetByCriteria_Async(
            dbCon,
            this.TermsData,
            this.SimplePostTagsData,
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

        return await this.SimplePostsData.GetCountByCriteria_Async( dbCon, parameters );
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

        SimplePostId simplePostId = await this.SimplePostsData.Create_Async(
            dbCon: dbCon,
            serverData: this.ServerData,
            userData: this.UserAppData,
            termsData: this.TermsData,
            termSetsData: this.SimplePostTagsData,
            termHistoryData: this.UserTermsHistoryData,
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
