using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess.Bundled;
using MindCabinet.Data;
using MindCabinet.Data.DataAccess;
using MindCabinet.Services;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.PostsContext;
using MindCabinet.Utility.Attributes;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;


namespace MindCabinet.Hubs;


[HubRoute( ClientDataAccess_ClientSessionBundle.IAPI.BaseRoute )]
public class SessionController(
            ILogger<SessionController> logger,
            DbAccess dbAccess,
            ServerDataAccess_SimpleUserSessions sessionsData,
			ClientSessionManager sessMngr
        ) : Hub, ClientDataAccess_ClientSessionBundle.IAPI {
    private readonly ILogger<SessionController> Logger = logger;

    private readonly DbAccess DbAccess = dbAccess;

    private readonly ServerDataAccess_SimpleUserSessions SessionsData = sessionsData;

    private readonly ClientSessionManager SessionManager = sessMngr;

    

    public async Task<ClientDataAccess_ClientSessionBundle.IAPI.GetCurrentDataBundle_Return> GetCurrent_Async( object _ ) {
        if( !this.SessionManager.IsLoaded ) {
            throw new NullReferenceException( "Session not loaded." );
        }
        if( this.SessionManager.CurrentSessionId is null ) {
            throw new NullReferenceException( "Session has no ID." );
        }

        var ret = new ClientDataAccess_ClientSessionBundle.IAPI.GetCurrentDataBundle_Return {
            SessionId = this.SessionManager.CurrentSessionId,
            UserData = this.SessionManager.UserOfSession is not null
                ? new SimpleUserObject.ClientObject(
                    id: this.SessionManager.UserOfSession.Id,
                    name: this.SessionManager.UserOfSession.Name,
                    created: this.SessionManager.UserOfSession.Created,
                    email: this.SessionManager.UserOfSession.Email
                )
                : null,
            UserAppData = this.SessionManager.UserAppDataOfSession?.ToRaw(),
            UserAppData_PostsContext = this.SessionManager.UserAppDataOfSession?.PostsContext?.ToRaw(),
            UserAppData_UserDefaultTerm = this.SessionManager.UserAppDataOfSession?.UserDefaultTerm?.ToRaw()
        };
//this.Logger.LogInformation( "SESS CTX "+JsonSerializer.Serialize(userAppData_PostsContext) );

        return ret;
    }


    public async Task<string> Logout_Async() {
        if( !this.SessionManager.IsLoaded ) {
            throw new NullReferenceException( "Session not loaded." );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        await this.SessionManager.LogoutSessionAndItsUser_Async( dbCon, this.SessionsData );

        return "Logout successful.";
    }
}
