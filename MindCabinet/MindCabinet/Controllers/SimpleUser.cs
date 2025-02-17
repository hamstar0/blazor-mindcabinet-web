using Microsoft.AspNetCore.Mvc;
using MindCabinet.Client.Data;
using MindCabinet.Data;
using MindCabinet.Shared.DataEntries;
using System.Data;


namespace MindCabinet;


[ApiController]
[Route("[controller]")]
public class SimpleUserController : ControllerBase {
    private SingletonCache Cache;
    private ServerDataAccess Data;



    public SimpleUserController( SingletonCache cache, ServerDataAccess data ) {
        this.Cache = cache;
        this.Data = data;
    }

    [HttpPost("Create")]
    public async Task<SimpleUserEntry> Create_Async( ClientDataAccess.CreateSimpleUserParams parameters ) {
        using IDbConnection dbCon = await this.Data.ConnectDb();

        return await this.Data.CreateSimpleUser_Async(
            dbCon,
            parameters,
            this.Cache.Users[ current user session id? ].PwSalt
        );
    }
}
