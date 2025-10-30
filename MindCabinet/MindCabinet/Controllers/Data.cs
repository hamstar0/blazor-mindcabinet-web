using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.WebEncoders.Testing;
using MindCabinet.Client.Services;
using MindCabinet.Data;
using MindCabinet.Shared.DataObjects;
using System.Data;


namespace MindCabinet;


[ApiController]
[Route("[controller]")]
public class DataController : ControllerBase {
    private readonly ServerDbAccess DbAccess;



    public DataController( ServerDbAccess dbAccess ) {
        this.DbAccess = dbAccess;
    }

    [HttpGet("Install")]
    public async Task<string> Install_Async() {
        using IDbConnection dbCon = await this.DbAccess.ConnectDb_Async( false );

        if( await this.DbAccess.Install_Async(dbCon) ) {
            return "Success";
        } else {
            return "Failure";
        }
    }
}
