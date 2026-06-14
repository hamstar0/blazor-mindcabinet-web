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


// [HubRoute( ClientDataAccess_SimpleUsers.IAPI.BaseRoute )]
// [Route("[controller]")]
[ApiController]
[Route( ClientDataAccess_SimpleUsers.IAPI.BaseRoute )]
public partial class SimpleUserController : ControllerBase, ClientDataAccess_SimpleUsers.IAPI {
    private readonly ILogger<SimpleUserController> Logger;

    private readonly IServiceProvider ServiceProvider;

    private readonly DbAccess DbAccess;

    private readonly ServerDataAccess_ServerData ServerDataSrc;

    private readonly ServerDataAccess_Terms TermsDataSrc;

    private readonly ServerDataAccess_SimpleUsers SimpleUsersDataSrc;

    private readonly ServerDataAccess_PostsContexts PostsContextDataSrc;

    private readonly ServerDataAccess_PostsContextTermEntry PostsContextTermEntryDataSrc;

    private readonly ServerDataAccess_SimpleUserSessions UserSessionsDataSrc;

    private readonly ServerDataAccess_UserTermFavorites FavoriteTermsDataSrc;

    private readonly ServerDataAccess_UserAppData UserAppDataSrc;
    
    private readonly ClientSessionManager SessionManager;



    public SimpleUserController(
                ILogger<SimpleUserController> logger,
                IServiceProvider serviceProvider,
                DbAccess dbAccess,
                ServerDataAccess_ServerData serverDataSrc,
                ServerDataAccess_Terms termsDataSrc,
                ServerDataAccess_SimpleUsers simpleUsersDataSrc,
                ServerDataAccess_PostsContexts postsContextDataSrc,
                ServerDataAccess_PostsContextTermEntry postsContextTermEntryDataSrc,
                ServerDataAccess_SimpleUserSessions userSessionsDataSrc,
                ServerDataAccess_UserTermFavorites favoriteTermsDataSrc,
                ServerDataAccess_UserAppData userAppDataSrc,
				ClientSessionManager sessMngr ) {
        this.Logger = logger;
        this.ServiceProvider = serviceProvider;
        this.DbAccess = dbAccess;
        this.ServerDataSrc = serverDataSrc;
        this.TermsDataSrc = termsDataSrc;
        this.SimpleUsersDataSrc = simpleUsersDataSrc;
        this.PostsContextDataSrc = postsContextDataSrc;
        this.PostsContextTermEntryDataSrc = postsContextTermEntryDataSrc;
        this.UserSessionsDataSrc = userSessionsDataSrc;
        this.FavoriteTermsDataSrc = favoriteTermsDataSrc;
        this.UserAppDataSrc = userAppDataSrc;
        this.SessionManager = sessMngr;
    }


    [HttpPost(nameof(Create_Async))]
    public async Task<ClientDataAccess_SimpleUsers.IAPI.Create_Return> Create_Async(
                ClientDataAccess_SimpleUsers.IAPI.Create_Params parameters ) {
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

        ServerDataAccess_SimpleUsers.SimpleUserQueryResult result = await this.SimpleUsersDataSrc.CreateSimpleUser_Async(
            dbCon: dbCon,
            serverDataSrc: this.ServerDataSrc,
            termsDataSrc: this.TermsDataSrc,
            postsContextDataSrc: this.PostsContextDataSrc,
            postsContextTermEntryDataSrc: this.PostsContextTermEntryDataSrc,
            userAppDataSrc: this.UserAppDataSrc,
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
