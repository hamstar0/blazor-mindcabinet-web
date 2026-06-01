using Microsoft.AspNetCore.Mvc;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Data;
using MindCabinet.Data.DataAccess;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsContext;
using System.Data;
using System.Text.Json;
using MindCabinet.Services;
using MindCabinet.Utility.Attributes;
using Microsoft.AspNetCore.SignalR;


namespace MindCabinet.Hubs;


[HubRoute( ClientDataAccess_PostsContext.IAPI.BaseRoute )]
public class PostsContextHub(
                ILogger<PostsContextHub> logger,
                IServiceProvider serviceProvider,
                DbAccess dbAccess,
                ServerDataAccess_PostsContexts postsContextsData,
                ServerDataAccess_PostsContextTermEntry postsContextTermEntryData,
				ClientSessionManager sessMngr
            ) : Hub, ClientDataAccess_PostsContext.IAPI {
    private readonly ILogger<PostsContextHub> Logger = logger;

    private readonly IServiceProvider ServiceProvider = serviceProvider;

    private readonly DbAccess DbAccess = dbAccess;

    private readonly ServerDataAccess_PostsContexts PostsContextsData = postsContextsData;

    private readonly ServerDataAccess_PostsContextTermEntry PostsContextTermEntryData = postsContextTermEntryData;

    private readonly ClientSessionManager SessionManager = sessMngr;



    //[HttpPost(ClientDataAccess_PostsContext.GetForCurrentUserByCriteria_Route)]
    public async Task<ClientDataAccess_PostsContext.IAPI.Get_Return> GetForCurrentUserByCriteria_Async(
                ClientDataAccess_PostsContext.IAPI.GetByCriteria_Params parameters ) {
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

        TermId currUserTermId = this.SessionManager.UserAppDataOfSession?.UserDefaultTerm.Id ?? 0;

        if( !parameters.TagTermIds.Any(tid => tid == currUserTermId) ) {
            parameters.TagTermIds = parameters.TagTermIds
                .Append( currUserTermId )
                .ToArray();
        }

        IEnumerable<PostsContextObject.Raw> contexts = await this.PostsContextsData.GetByCriteria_Async(
            dbCon: dbCon,
            postsContextTermEntryData: this.PostsContextTermEntryData,
            parameters: parameters,
            alsoGetEntries: true
        );

        return new ClientDataAccess_PostsContext.IAPI.Get_Return { Contexts = contexts };
    }

    public async Task<ClientDataAccess_PostsContext.IAPI.CreateOrUpdate_Return> CreateForCurrentUser_Async(
                PostsContextObject.Prototype parameters ) {
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
        if( !parameters.IsValid(false) ) {
            throw new ArgumentException( "Invalid PostsContextObject.Prototype in parameters." );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        return await this.PostsContextsData.Create_Async(
            dbCon,
            this.PostsContextTermEntryData,
            parameters
        );
    }

    public async Task<ClientDataAccess_PostsContext.IAPI.CreateOrUpdate_Return> UpdateForCurrentUser_Async(
                PostsContextObject.Prototype parameters ) {
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
        if( parameters.Id == 0 ) {
            throw new ArgumentException( "PostsContextObject.Prototype Id is not valid (must be non-null and non-zero)." );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        return await this.PostsContextsData.Update_Async(
            dbCon: dbCon,
            postsContextTermEntryData: this.PostsContextTermEntryData,
            parameters: parameters
        );
    }
}
