using Microsoft.AspNetCore.Mvc;
using MindCabinet.Client.Data;
using MindCabinet.Data;
using MindCabinet.Shared.DataEntries;
using System.Data;


namespace MindCabinet;


[ApiController]
[Route("[controller]")]
public class TermController : ControllerBase {
    private readonly ServerDataAccess Data;



    public TermController( ServerDataAccess data ) {
        this.Data = data;
    }


    [HttpPost("GetByCriteria")]
    public async Task<IEnumerable<TermEntry>> GetByCriteria_Async(
                ClientDataAccess.GetTermsByCriteriaParams parameters ) {
        using IDbConnection dbCon = await this.Data.ConnectDb();

        return await this.Data.GetTermsByCriteria_Async( dbCon, parameters );
    }

    [HttpPost("Create")]
    public async Task<ClientDataAccess.CreateTermReturn> Create_Async( ClientDataAccess.CreateTermParams parameters ) {
        using IDbConnection dbCon = await this.Data.ConnectDb();

        return await this.Data.CreateTerm_Async( dbCon, parameters );
    }
}
