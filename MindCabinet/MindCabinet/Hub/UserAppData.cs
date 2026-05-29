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


namespace MindCabinet.Hubs;


[HubRoute( ClientDataAccess_UserAppData.IAPI.BaseRoute )]
public class UserAppDataController : Hub, ClientDataAccess_UserAppData.IAPI {
    private readonly DbAccess DbAccess;

    private readonly ServerDataAccess_UserAppData UserAppData;

    private readonly ClientSessionManager ServerSessionManager;



    public UserAppDataController(
                DbAccess dbAccess,
                ServerDataAccess_UserAppData userAppData,
                ClientSessionManager serverSessionManager ) {
        this.DbAccess = dbAccess;
        this.UserAppData = userAppData;
        this.ServerSessionManager = serverSessionManager;
    }


    public async Task<ClientDataAccess_UserAppData.IAPI.GetForCurrentUser_Return> GetForCurrentUser_Async( object _ ) {
        if( this.ServerSessionManager.UserOfSession is null ) {
            throw new InvalidOperationException( "No current user available." );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        UserAppDataObject.Raw? userAppDataRaw = await this.UserAppData.GetById_Async(
            dbCon,
            this.ServerSessionManager.UserOfSession?.Id ?? 0
        );
        if( userAppDataRaw is null ) {
            throw new Exception( "User app data missing for user." );
        }

        return new ClientDataAccess_UserAppData.IAPI.GetForCurrentUser_Return {
            UserAppData = userAppDataRaw
        };
    }

    public async Task<object> UpdateForCurrentUser_Async(
                UserAppDataObject.Prototype parameters ) {
        if( this.ServerSessionManager.UserOfSession is null ) {
            throw new InvalidOperationException( "No current user available." );
        }
        if( !parameters.IsValidAsObject(true) ) {
            throw new InvalidOperationException( "Invalid parameters." );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        await this.UserAppData.Update_Async(
            dbCon: dbCon,
            simpleUserId: this.ServerSessionManager.UserOfSession.Id,
            postsContextId: parameters.PostsContextId ?? 0,
            userDefaultTermId: parameters.UserDefaultTermId ?? 0
        );

        return new {};
    }
}
