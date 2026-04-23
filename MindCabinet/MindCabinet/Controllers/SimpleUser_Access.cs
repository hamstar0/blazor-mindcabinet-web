using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Data;
using MindCabinet.Data.DataAccess;
using MindCabinet.Shared.DataObjects;
using System.Data;
using System.Security.Cryptography;
using System.Text;


namespace MindCabinet.Controllers;


public partial class SimpleUserController : ControllerBase {
    [HttpPost(ClientDataAccess_SimpleUsers.Login_Route)]
    public async Task<ClientDataAccess_SimpleUsers.Login_Return> Login_Async(
                ClientDataAccess_SimpleUsers.Login_Params parameters ) {
        if( !this.ServerSessionData.IsLoaded ) {
            throw new NullReferenceException( "Session not loaded." );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        SimpleUserObject.Raw? userRaw = await this.SimpleUsersData.GetByName_Async( dbCon, parameters.Name );
        if( userRaw is null ) {
            return new ClientDataAccess_SimpleUsers.Login_Return { User = null, Status = "User not found by name: "+parameters.Name };
        }

        byte[] pwHash = ServerDataAccess_SimpleUsers.GeneratePasswordHash( parameters.Password, userRaw.PwSalt );

// this.Logger.LogInformation( "pw: "+parameters.Password
// +", user.PwHash: "+Encoding.UTF8.GetString(user.PwHash)
// +", pwHash: "+Encoding.UTF8.GetString(pwHash) );
        if( !CryptographicOperations.FixedTimeEquals(userRaw.PwHash, pwHash) ) {
            return new ClientDataAccess_SimpleUsers.Login_Return { User = null, Status = "Invalid password." };
        }

        await this.UserSessionsData.Create_Async( dbCon, userRaw.Id, this.ServerSessionData );
        //await this.SessionsData.VisitSimpleUserSession_Async( dbCon, this.ServerSessionData );

        return new ClientDataAccess_SimpleUsers.Login_Return {
            User = new SimpleUserObject.ClientObject( userRaw.Id, userRaw.Name, userRaw.Created, userRaw.Email ),
            Status = "User validated."
        };
    }

    [HttpPost("Visit")]
    public async Task Visit_Async() {
        if( !this.ServerSessionData.IsLoaded || this.ServerSessionData.UserOfSession is null ) {
            return;
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        await this.UserSessionsData.VisitSimpleUserSession_Async( dbCon, this.ServerSessionData );
    }
}
