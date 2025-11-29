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


[ApiController]
[Route("[controller]")]
public partial class SimpleUserController : ControllerBase {
    private ServerDbAccess DbAccess;
    private ServerSessionData SessData;



    public SimpleUserController( ServerDbAccess dbAccess, ServerSessionData sessData ) {
        this.DbAccess = dbAccess;
        this.SessData = sessData;
    }

    [HttpPost(ClientDbAccess_SimpleUsers.Create_Route)]
    public async Task<ClientDbAccess_SimpleUsers.Login_Return> Create_Async(
                ClientDbAccess_SimpleUsers.Create_Params parameters ) {
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

        return new ClientDbAccess_SimpleUsers.Login_Return(
            result.User?.GetClientOnlyData(),
            result.User is not null ? "User created." : "Could not create user."
        );
    }
}
