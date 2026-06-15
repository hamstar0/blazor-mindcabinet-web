using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Data;
using MindCabinet.Data.DataAccess;
using MindCabinet.Services;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Utility.Attributes;
using System.Data;


namespace MindCabinet.Controllers;


// [HubRoute( ClientDataAccess_UserAppData.IAPI.BaseRoute )]
// [Route("[controller]")]
[ApiController]
[Route( ClientDataAccess_UserAppData.IAPI.BaseRoute )]
public class UserAppDataController : ControllerBase, ClientDataAccess_UserAppData.IAPI {
    private readonly DbAccess DbAccess;

    private readonly IServiceProvider ServiceProvider;

    private readonly ServerDataAccess_UserAppData UserAppDataSrc;

    private readonly ClientSessionManager SessionManager;



    public UserAppDataController(
                DbAccess dbAccess,
                IServiceProvider serviceProvider,
                ServerDataAccess_UserAppData userAppDataSrc,
                ClientSessionManager serverSessionManager ) {
        this.DbAccess = dbAccess;
        this.ServiceProvider = serviceProvider;
        this.UserAppDataSrc = userAppDataSrc;
        this.SessionManager = serverSessionManager;
    }


    [HttpPost(nameof(GetForCurrentUser_Async))]
    public async Task<ClientDataAccess_UserAppData.IAPI.GetForCurrentUser_Return> GetForCurrentUser_Async( object _ ) {
        if( this.SessionManager.UserOfSession is null ) {
            throw new InvalidOperationException( "No current user available." );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        UserAppDataObject.Raw? userAppDataRaw = await this.UserAppDataSrc.GetById_Async(
            dbCon,
            this.SessionManager.UserOfSession?.Id ?? 0
        );
        if( userAppDataRaw is null ) {
            throw new Exception( "User app data missing for user." );
        }

        return new ClientDataAccess_UserAppData.IAPI.GetForCurrentUser_Return {
            UserAppData = userAppDataRaw
        };
    }


    [HttpPost(nameof(UpdateForCurrentUser_Async))]
    public async Task<object> UpdateForCurrentUser_Async(
                UserAppDataObject.Prototype parameters ) {
        if( this.SessionManager.UserOfSession is null ) {
            throw new InvalidOperationException( "No current user available." );
        }
        if( !parameters.IsValidAsObject(true) ) {
            throw new InvalidOperationException( "Invalid parameters." );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        await this.UserAppDataSrc.Update_Async(
            dbCon: dbCon,
            simpleUserId: this.SessionManager.UserOfSession.Id,
            postsContextId: parameters.CurrentPostsContextId ?? 0,
            userDefaultTermId: parameters.UserDefaultTermId ?? 0
        );

        return new {};
    }
}
