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



    public SimpleUserController( ServerDbAccess dbAccess, ServerSessionData sessData ) {    //IDistributedCache cache
        this.DbAccess = dbAccess;
        this.SessData = sessData;

        //if( cache.Get("PwSalt") is null ) {
        //    cache.Set(
        //        "PwSalt",
        //        Encoding.ASCII.GetBytes( SimpleUserEntry.GeneratePwSalt() ),
        //        new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = new TimeSpan(0, 30, 0) }
        //    );
        //}
    }

    [HttpPost("Create")]
    public async Task<SimpleUserEntry> Create_Async( ClientDbAccess.CreateSimpleUserParams parameters ) {
        if( this.SessData.PwSalt is null ) {
            throw new NullReferenceException( "No password salt for current session." );
        }
        
        using IDbConnection dbCon = await this.DbAccess.ConnectDb();

        return await this.DbAccess.CreateSimpleUser_Async(
            dbCon: dbCon,
            parameters: parameters,
            pwSalt: this.SessData.PwSalt
            //this.Cache.Users[ current user session id? ].PwSalt
        );
    }
}
