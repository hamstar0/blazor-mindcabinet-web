using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Data;
using MindCabinet.Data.DataAccess;
using MindCabinet.Services;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Utility.Attributes;
using System.Data;
using System.Security.Cryptography;
using System.Text;


namespace MindCabinet.Hubs;


[HubRoute( ClientDataAccess_SimpleUsers.IAPI.BaseRoute )]
public partial class SimpleUserController : Hub, ClientDataAccess_SimpleUsers.IAPI {
    private readonly ILogger<SimpleUserController> Logger;

    private readonly IServiceProvider ServiceProvider;

    private readonly DbAccess DbAccess;

    private readonly ServerDataAccess_ServerData ServerData;

    private readonly ServerDataAccess_Terms TermsData;

    private readonly ServerDataAccess_SimpleUsers SimpleUsersData;

    private readonly ServerDataAccess_PostsContexts PostsContextData;

    private readonly ServerDataAccess_PostsContextTermEntry PostsContextTermEntryData;

    private readonly ServerDataAccess_SimpleUserSessions UserSessionsData;

    private readonly ServerDataAccess_UserTermFavorites FavoriteTermsData;

    private readonly ServerDataAccess_UserAppData UserAppData;
    
    private readonly ClientSessionManager SessionManager;



    public SimpleUserController(
                ILogger<SimpleUserController> logger,
                IServiceProvider serviceProvider,
                DbAccess dbAccess,
                ServerDataAccess_ServerData serverData,
                ServerDataAccess_Terms termsData,
                ServerDataAccess_SimpleUsers simpleUsersData,
                ServerDataAccess_PostsContexts postsContextData,
                ServerDataAccess_PostsContextTermEntry postsContextTermEntryData,
                ServerDataAccess_SimpleUserSessions userSessionsData,
                ServerDataAccess_UserTermFavorites favoriteTermsData,
                ServerDataAccess_UserAppData userAppData,
				ClientSessionManager sessMngr ) {
        this.Logger = logger;
        this.ServiceProvider = serviceProvider;
        this.DbAccess = dbAccess;
        this.ServerData = serverData;
        this.TermsData = termsData;
        this.SimpleUsersData = simpleUsersData;
        this.PostsContextData = postsContextData;
        this.PostsContextTermEntryData = postsContextTermEntryData;
        this.UserSessionsData = userSessionsData;
        this.FavoriteTermsData = favoriteTermsData;
        this.UserAppData = userAppData;
        this.SessionManager = sessMngr;
    }


    public async Task<ClientDataAccess_SimpleUsers.IAPI.Create_Return> Create_Async(
                ClientDataAccess_SimpleUsers.IAPI.Create_Params parameters ) {
        if( !this.SessionManager.IsLoaded ) {
            HttpContext? context = this.Context.GetHttpContext();
            if( context is null ) {
                throw new InvalidOperationException( $"No HttpContext in {this.GetType().Name}" );
            }
            await ClientSessionManager.LoadForHubRequest_Async( this.ServiceProvider );
        }
        
        if( !this.SessionManager.IsLoaded ) {
            throw new NullReferenceException( "Session not loaded." );
        }
        if( this.SessionManager.UserOfSession is not null ) {
            return new ClientDataAccess_SimpleUsers.IAPI.Create_Return { User = null, Status = "User already in session" };
        }
        if( parameters.IsValidated ) {
            return new ClientDataAccess_SimpleUsers.IAPI.Create_Return { User = null, Status = "Not permitted." };   // maybe watch this guy from now on
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        ServerDataAccess_SimpleUsers.SimpleUserQueryResult result = await this.SimpleUsersData.CreateSimpleUser_Async(
            dbCon: dbCon,
            serverData: this.ServerData,
            termsData: this.TermsData,
            postsContextData: this.PostsContextData,
            postsContextTermEntryData: this.PostsContextTermEntryData,
            userAppData: this.UserAppData,
            parameters: parameters,
            detectCollision: true,
            createPostsContext: true
        );

        // if( result.User is not null ) {      <- Do not log in automatically!
        //     await this.SessionsData.CreateSimpleUserSession_Async(
        //         dbCon: dbCon,
        //         user: result.User,
        //         session: this.ServerSessionData
        //     );
        // }
        this.Logger.LogInformation( $"User already exists? {result.AlreadyExists}" );

        return new ClientDataAccess_SimpleUsers.IAPI.Create_Return {
            User = result.User is not null
                ? new SimpleUserObject.ClientObject( result.User.Id, result.User.Name, result.User.Created, result.User.Email )
                : null,
            Status = result.User is not null
                ? "User created. Validate email address and log in to complete registration."
                : "Could not create user."
        };
    }
}
