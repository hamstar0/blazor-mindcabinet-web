using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using MindCabinet.Client.Services;
using MindCabinet.Data;
using MindCabinet.Shared.DataEntries;
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
    public async Task<ClientSessionData.JsonData> GetSessionData_Async() {
        if( !this.SessData.IsLoaded ) {
            throw new NullReferenceException( "Session not loaded." );
        }
        
        using IDbConnection dbCon = await this.DbAccess.ConnectDb();

        return new ClientSessionData.JsonData( this.SessData.SessionId! );
    }

    [HttpPost("Create")]
    public async Task<ServerDbAccess.SimpleUserQueryResult> Create_Async(
                ClientDbAccess.CreateSimpleUserParams parameters ) {
        if( !this.SessData.IsLoaded ) {
            throw new NullReferenceException( "Session not loaded." );
        }

        using IDbConnection dbCon = await this.DbAccess.ConnectDb();

        return await this.DbAccess.CreateSimpleUser_Async(
            dbCon: dbCon,
            parameters: parameters,
            pwSalt: this.SessData.PwSalt!
        );
    }
}
