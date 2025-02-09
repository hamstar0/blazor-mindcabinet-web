using Microsoft.AspNetCore.Mvc;
using MindCabinet.Client.Data;
using MindCabinet.Data;
using MindCabinet.Shared.DataEntries;
using System.Data;


namespace MindCabinet;


[ApiController]
[Route("[controller]")]
public class DataController : ControllerBase {
    private readonly ServerDataAccess Data;



    public DataController( ServerDataAccess data ) {
        this.Data = data;
    }


    [HttpGet("Install")]
    public async Task<string> Install_Async() {
        using IDbConnection dbCon = await this.Data.ConnectDb( false );

        if( await this.Data.Install_Async(dbCon) ) {
            return "Success";
        } else {
            return "Failure";
        }
    }
}
