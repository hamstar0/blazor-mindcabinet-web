using Microsoft.AspNetCore.Mvc;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Data;
using MindCabinet.Data.DataAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using System.Data;


namespace MindCabinet.Controllers;


[ApiController]
[Route("[controller]")]
public class UserAppDataController : ControllerBase {
    private readonly DbAccess DbAccess;

    private readonly ServerDataAccess_UserAppData UserAppData;

    private readonly ServerSessionData ServerSessionData;



    public UserAppDataController(
                DbAccess dbAccess,
                ServerDataAccess_UserAppData userAppData,
                ServerSessionData serverSessionData ) {
        this.DbAccess = dbAccess;
        this.UserAppData = userAppData;
        this.ServerSessionData = serverSessionData;
    }


    [HttpPost(ClientDataAccess_UserAppData.GetForCurrentUser_Route)]
    public async Task<ClientDataAccess_UserAppData.GetForCurrentUser_Return> GetForCurrentUser_Async( object _ ) {
        if( this.ServerSessionData.UserOfSession is null ) {
            throw new InvalidOperationException( "No current user available." );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        UserAppDataObject.Raw? userAppDataRaw = await this.UserAppData.GetById_Async(
            dbCon,
            this.ServerSessionData.UserOfSession?.Id ?? 0
        );
        if( userAppDataRaw is null ) {
            throw new Exception( "User app data missing for user." );
        }

        return new ClientDataAccess_UserAppData.GetForCurrentUser_Return {
            UserAppData = userAppDataRaw
        };
    }

    [HttpPost(ClientDataAccess_UserAppData.UpdateForCurrentUser_Route)]
    public async Task<object> UpdateForCurrentUser_Async(
                UserAppDataObject.Prototype parameters ) {
        if( this.ServerSessionData.UserOfSession is null ) {
            throw new InvalidOperationException( "No current user available." );
        }
        if( !parameters.IsValidAsObject(true) ) {
            throw new InvalidOperationException( "Invalid parameters." );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        await this.UserAppData.Update_Async(
            dbCon: dbCon,
            simpleUserId: this.ServerSessionData.UserOfSession.Id,
            postsContextId: parameters.PostsContextId ?? 0
        );

        return new {};
    }
}
