using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using MindCabinet.Client.Services;
using MindCabinet.Data;
using MindCabinet.Shared.DataObjects;
using System.Data;
using System.Text;


namespace MindCabinet;


[ApiController]
[Route("[controller]")]
public class SimpleUserController : ControllerBase {
    private ServerDbAccess DbAccess;
    private ServerSessionData SessData;



    public SimpleUserController( ServerDbAccess dbAccess, ServerSessionData sessData ) {
        this.DbAccess = dbAccess;
        this.SessData = sessData;
    }

    [HttpPost("GetSessionData")]
    public async Task<ClientSessionData.RawData> GetSessionData_Async() {
        if( !this.SessData.IsLoaded ) {
            throw new NullReferenceException( "Session not loaded." );
        }

        return new ClientSessionData.RawData(
            sessionId: this.SessData.SessionId!,
            userData: this.SessData.User?.GetClientOnlyData()
        );
    }

    [HttpPost("Create")]
    public async Task<ClientDbAccess.SimpleUserLoginReply> Create_Async(
                ClientDbAccess.CreateSimpleUserParams parameters ) {
        if( !this.SessData.IsLoaded ) {
            throw new NullReferenceException( "Session not loaded." );
        }

        using IDbConnection dbCon = await this.DbAccess.ConnectDb_Async();

        ServerDbAccess.SimpleUserQueryResult result = await this.DbAccess.CreateSimpleUser_Async(
            dbCon: dbCon,
            parameters: parameters,
            pwSalt: this.SessData.PwSalt!
        );

        if( result.User is not null ) {
            await this.DbAccess.CreateSimpleUserSession_Async(
                dbCon: dbCon,
                user: result.User,
                session: this.SessData
            );
        }

        return new ClientDbAccess.SimpleUserLoginReply(
            result.User?.GetClientOnlyData(),
            result.User is not null ? "User created." : "Could not create user."
        );
    }

    [HttpPost("Login")]
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

        if( !Enumerable.SequenceEqual(user.PwHash, pwHash) ) {
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
