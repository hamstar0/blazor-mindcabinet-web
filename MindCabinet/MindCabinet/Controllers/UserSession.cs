using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
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


namespace MindCabinet.Controllers;


[ApiController]
[Route( ClientDataAccess_UserSession.IAPI.BaseRoute )]
public class UserSessionController(
                ILogger<UserSessionController> logger,
                IServiceProvider serviceProvider,
                DbAccess dbAccess,
                ServerDataAccess_SimpleUserSessions sessionsDataSrc,
                ClientSessionManager sessMngr
            ) : ControllerBase, ClientDataAccess_UserSession.IAPI {
    private readonly ILogger<UserSessionController> Logger = logger;

    private readonly IServiceProvider ServiceProvider = serviceProvider;

    private readonly DbAccess DbAccess = dbAccess;

    private readonly ServerDataAccess_SimpleUserSessions SessionsDataSrc = sessionsDataSrc;

    private readonly ClientSessionManager SessionManager = sessMngr;



	[HttpPost(nameof(Logout_Async))]
    public async Task<object> Logout_Async( int _ ) {
        if( !this.SessionManager.IsLoaded ) {
            throw new NullReferenceException( "Session not loaded." );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        await this.SessionManager.LogoutSessionAndItsUser_Async( dbCon, this.SessionsDataSrc );

        return new {};
    }
}
