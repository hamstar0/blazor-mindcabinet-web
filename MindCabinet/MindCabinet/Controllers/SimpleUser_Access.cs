using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Data;
using MindCabinet.Data.DataAccess;
using MindCabinet.Services;
using MindCabinet.Shared.DataObjects;
using System.Data;
using System.Security.Cryptography;
using System.Text;


namespace MindCabinet.Hubs;


public partial class SimpleUserController : ControllerBase, ClientDataAccess_SimpleUsers.IAPI {
    [HttpPost(nameof(Login_Async))]
    public async Task<ClientDataAccess_SimpleUsers.IAPI.Login_Return> Login_Async(
                ClientDataAccess_SimpleUsers.IAPI.Login_Params parameters ) {
        if( !this.SessionManager.IsLoaded ) {
            throw new NullReferenceException( "Session not loaded." );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        SimpleUserObject.Raw? userRaw = await this.SimpleUsersDataSrc.GetByName_Async( dbCon, parameters.Name );
        if( userRaw is null ) {
            // return new ClientDataAccess_SimpleUsers.Login_Return { User = null, Status = "User not found by name: "+parameters.Name };
            return new ClientDataAccess_SimpleUsers.IAPI.Login_Return {
                User = null,
                Status = "User name or password invalid."
            };
        }

        byte[] pwHash = ServerDataAccess_SimpleUsers.GeneratePasswordHash( parameters.Password, userRaw.PwSalt );

// this.Logger.LogInformation( "pw: "+parameters.Password
// +", user.PwHash: "+Encoding.UTF8.GetString(user.PwHash)
// +", pwHash: "+Encoding.UTF8.GetString(pwHash) );
        if( !CryptographicOperations.FixedTimeEquals(userRaw.PwHash, pwHash) ) {
            return new ClientDataAccess_SimpleUsers.IAPI.Login_Return {
                User = null,
                Status = "User name or password invalid."
            };
        }

        await this.UserSessionsDataSrc.Create_Async( dbCon, this.SessionManager, userRaw.Id );
        //await this.SessionsData.VisitSimpleUserSession_Async( dbCon, this.ServerSessionData );

        return new ClientDataAccess_SimpleUsers.IAPI.Login_Return {
            User = new SimpleUserObject.ClientObject( userRaw.Id, userRaw.Name, userRaw.Created, userRaw.Email ),
            Status = "User validated."
        };
    }
    

    [HttpPost(nameof(Visit_Async))]
    public async Task Visit_Async() {
        if( !this.SessionManager.IsLoaded || this.SessionManager.UserOfSession is null ) {
            return;
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        await this.UserSessionsDataSrc.VisitSimpleUserSession_Async( dbCon, this.SessionManager );
    }
}
