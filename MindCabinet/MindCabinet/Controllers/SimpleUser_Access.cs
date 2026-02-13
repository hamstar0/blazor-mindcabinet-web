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

        SimpleUserObject? user = await this.SimpleUsersData.GetSimpleUser_Async( dbCon, parameters.Name );
        if( user is null ) {
            return new ClientDataAccess_SimpleUsers.Login_Return( null, "User not found by name: "+parameters.Name );
        }

        byte[] pwHash = ServerDataAccess_SimpleUsers.GeneratePasswordHash( parameters.Password, user.PwSalt );

// this.Logger.LogInformation( "pw: "+parameters.Password
// +", user.PwHash: "+Encoding.UTF8.GetString(user.PwHash)
// +", pwHash: "+Encoding.UTF8.GetString(pwHash) );
        if( !CryptographicOperations.FixedTimeEquals(user.PwHash, pwHash) ) {
            return new ClientDataAccess_SimpleUsers.Login_Return( null, "Invalid password." );
        }

        await this.SessionsData.CreateSimpleUserSession_Async( dbCon, user, this.ServerSessionData );
        //await this.SessionsData.VisitSimpleUserSession_Async( dbCon, this.ServerSessionData );

        return new ClientDataAccess_SimpleUsers.Login_Return( user.GetClientOnlyData(), "User validated." );
    }

    [HttpPost("Visit")]
    public async Task Visit_Async() {
        if( !this.ServerSessionData.IsLoaded || this.ServerSessionData.User is null ) {
            return;
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        await this.SessionsData.VisitSimpleUserSession_Async( dbCon, this.ServerSessionData );
    }
}
