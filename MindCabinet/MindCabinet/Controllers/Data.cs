using Microsoft.AspNetCore.Mvc;
using MindCabinet.Client.Data;
using MindCabinet.Data;
using MindCabinet.Shared.DataEntries;


namespace MindCabinet;


[ApiController]
[Route("[controller]")]
public class DataController : ControllerBase {
    private readonly ServerDataAccess Data;



    public DataController( ServerDataAccess data ) {
        this.Data = data;
    }


    [HttpGet("Install")]
    public async Task<string> GetByCriteria_Async() {
        if( await this.Data.Install_Async() ) {
            return "Success";
        } else {
            return "Failure";
        }
    }
}
