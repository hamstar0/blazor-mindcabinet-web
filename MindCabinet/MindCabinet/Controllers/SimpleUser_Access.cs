using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using MindCabinet.Client.Services;
using MindCabinet.Data;
using MindCabinet.Shared.DataObjects;
using System.Data;
using System.Security.Cryptography;
using System.Text;


namespace MindCabinet;


public partial class SimpleUserController : ControllerBase {
    [HttpPost(ClientDbAccess.SimpleUser_Login_Route)]
    public async Task<ClientDbAccess.SimpleUserLoginReply> Login_Async(
                ClientDbAccess.LoginSimpleUserParams parameters ) {
        if( !this.SessData.IsLoaded ) {
            throw new NullReferenceException( "Session not loaded." );
        }

        using IDbConnection dbCon = await this.DbAccess.ConnectDb_Async();

        var user = await this.DbAccess.GetSimpleUser_Async( dbCon, parameters.Name );
        if( user is null ) {
            return new ClientDbAccess.SimpleUserLoginReply( null, "User not found by name: "+parameters.Name );
        }

        byte[] pwHash = ServerDbAccess.GetPasswordHash( parameters.Password, this.SessData.PwSalt );

        if( !CryptographicOperations.FixedTimeEquals(user.PwHash, pwHash) ) {
            return new ClientDbAccess.SimpleUserLoginReply( null, "Invalid password." );
        }

        await this.DbAccess.VisitSimpleUserSession_Async( dbCon, this.SessData );

        return new ClientDbAccess.SimpleUserLoginReply( user.GetClientOnlyData(), "User validated." );
    }

    [HttpPost("Visit")]
    public async Task Visit_Async() {
        if( !this.SessData.IsLoaded || this.SessData.User is null ) {
            return;
        }

        using IDbConnection dbCon = await this.DbAccess.ConnectDb_Async();

        await this.DbAccess.VisitSimpleUserSession_Async( dbCon, this.SessData );
    }
}
