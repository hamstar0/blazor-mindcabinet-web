using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Data;
using MindCabinet.Shared.DataObjects;
using System.Data;
using System.Security.Cryptography;
using System.Text;


namespace MindCabinet;


public partial class SimpleUserController : ControllerBase {
    [HttpPost(ClientDbAccess_SimpleUsers.Login_Route)]
    public async Task<ClientDbAccess_SimpleUsers.Login_Return> Login_Async(
                ClientDbAccess_SimpleUsers.Login_Params parameters ) {
        if( !this.SessData.IsLoaded ) {
            throw new NullReferenceException( "Session not loaded." );
        }

        using IDbConnection dbCon = await this.DbAccess.ConnectDb_Async();

        var user = await this.DbAccess.GetSimpleUser_Async( dbCon, parameters.Name );
        if( user is null ) {
            return new ClientDbAccess_SimpleUsers.Login_Return( null, "User not found by name: "+parameters.Name );
        }

        byte[] pwHash = ServerDbAccess.GetPasswordHash( parameters.Password, this.SessData.PwSalt );

        if( !CryptographicOperations.FixedTimeEquals(user.PwHash, pwHash) ) {
            return new ClientDbAccess_SimpleUsers.Login_Return( null, "Invalid password." );
        }

        await this.DbAccess.VisitSimpleUserSession_Async( dbCon, this.SessData );

        return new ClientDbAccess_SimpleUsers.Login_Return( user.GetClientOnlyData(), "User validated." );
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
