using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess.Bundled;
using MindCabinet.Data;
using MindCabinet.Data.DataAccess;
using MindCabinet.Shared.DataObjects;
using System.Data;
using System.Security.Cryptography;
using System.Text;


namespace MindCabinet.Controllers;


[ApiController]
[Route("[controller]")]
public class SessionController(
            ILogger<SessionController> logger,
            DbAccess dbAccess,
            ServerDataAccess_SimpleUserSessions sessionsData,
            ServerSessionData sessData
        ) : ControllerBase {
    private readonly ILogger<SessionController> Logger = logger;

    private readonly DbAccess DbAccess = dbAccess;

    private readonly ServerDataAccess_SimpleUserSessions SessionsData = sessionsData;

    private readonly ServerSessionData SessData = sessData;

    

    [HttpPost(ClientDataAccess_ClientSessionBundle.GetCurrent_Route)]
    public async Task<ClientDataAccess_ClientSessionBundle.GetCurrent_Return> GetCurrent_Async( object parameters ) {
        if( !this.SessData.IsLoaded ) {
            throw new NullReferenceException( "Session not loaded." );
        }
        if( this.SessData.SessionId is null ) {
            throw new NullReferenceException( "Session has no ID." );
        }

        return new ClientDataAccess_ClientSessionBundle.GetCurrent_Return {
            SessionId = this.SessData.SessionId,
            UserData = this.SessData.UserOfSession is not null
                ? new SimpleUserObject.ClientObject(
                    id: this.SessData.UserOfSession.Id,
                    name: this.SessData.UserOfSession.Name,
                    created: this.SessData.UserOfSession.Created,
                    email: this.SessData.UserOfSession.Email
                )
                : null,
            UserAppData = this.SessData.UserAppData?.ToRaw(),
            UserAppData_UserContext = this.SessData.UserAppData?.UserContext?.ToRaw()
        };
    }



    [HttpGet(ClientSessionData.Logout_Route)]
    public async Task<string> Logout_Async() {
        if( !this.SessData.IsLoaded ) {
            throw new NullReferenceException( "Session not loaded." );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        await this.SessData.LogoutSessionAndItsUser_Async( dbCon, this.SessionsData );

        return "Logout successful.";
    }
}
