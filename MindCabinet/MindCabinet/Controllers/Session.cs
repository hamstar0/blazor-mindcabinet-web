using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess.Bundled;
using MindCabinet.Data;
using MindCabinet.Data.DataAccess;
using MindCabinet.Services;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.PostsContext;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;


namespace MindCabinet.Controllers;


[ApiController]
[Route("[controller]")]
public class SessionController(
            ILogger<SessionController> logger,
            DbAccess dbAccess,
            ServerDataAccess_SimpleUserSessions sessionsData,
            ServerSessionManager sessMngr
        ) : ControllerBase {
    private readonly ILogger<SessionController> Logger = logger;

    private readonly DbAccess DbAccess = dbAccess;

    private readonly ServerDataAccess_SimpleUserSessions SessionsData = sessionsData;

    private readonly ServerSessionManager SessionManager = sessMngr;

    

    [HttpPost(ClientDataAccess_ClientSessionBundle.GetCurrent_Route)]
    public async Task<ClientDataAccess_ClientSessionBundle.GetCurrentDataBundle_Return> GetCurrent_Async( object _ ) {
        if( !this.SessionManager.IsLoaded ) {
            throw new NullReferenceException( "Session not loaded." );
        }
        if( this.SessionManager.CurrentSessionId is null ) {
            throw new NullReferenceException( "Session has no ID." );
        }

        UserAppDataObject.Raw? userAppData = this.SessionManager.UserAppDataOfSession?.ToRaw();

        PostsContextObject.Raw? userAppData_PostsContext = this.SessionManager.UserAppDataOfSession?.PostsContext?.ToRaw();
        
        var ret = new ClientDataAccess_ClientSessionBundle.GetCurrentDataBundle_Return {
            SessionId = this.SessionManager.CurrentSessionId,
            UserData = this.SessionManager.UserOfSession is not null
                ? new SimpleUserObject.ClientObject(
                    id: this.SessionManager.UserOfSession.Id,
                    name: this.SessionManager.UserOfSession.Name,
                    created: this.SessionManager.UserOfSession.Created,
                    email: this.SessionManager.UserOfSession.Email
                )
                : null,
            UserAppData = userAppData,
            UserAppData_PostsContext = userAppData_PostsContext
        };
//this.Logger.LogInformation( "SESS CTX "+JsonSerializer.Serialize(userAppData_PostsContext) );

        return ret;
    }


    [HttpGet(ClientSessionManager.Logout_Route)]
    public async Task<string> Logout_Async() {
        if( !this.SessionManager.IsLoaded ) {
            throw new NullReferenceException( "Session not loaded." );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        await this.SessionManager.LogoutSessionAndItsUser_Async( dbCon, this.SessionsData );

        return "Logout successful.";
    }
}
