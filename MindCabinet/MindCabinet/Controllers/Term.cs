using Microsoft.AspNetCore.Mvc;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Data;
using MindCabinet.Shared.DataObjects.Term;
using System.Data;


namespace MindCabinet;


[ApiController]
[Route("[controller]")]
public class TermController : ControllerBase {
    private readonly ServerDbAccess DbAccess;



    public TermController( ServerDbAccess dbAccess ) {
        this.DbAccess = dbAccess;
    }


    [HttpPost(ClientDbAccess_Terms.GetByCriteria_Route)]
    public async Task<IEnumerable<TermObject>> GetByCriteria_Async(
                ClientDbAccess_Terms.GetByCriteria_Params parameters ) {
        using IDbConnection dbCon = await this.DbAccess.ConnectDb_Async();

        return await this.DbAccess.GetTermsByCriteria_Async( dbCon, parameters );
    }

    [HttpPost(ClientDbAccess_Terms.Create_Route)]
    public async Task<ClientDbAccess_Terms.Create_Return> Create_Async(
                ClientDbAccess_Terms.Create_Params parameters ) {
        using IDbConnection dbCon = await this.DbAccess.ConnectDb_Async();

        return await this.DbAccess.CreateTerm_Async( dbCon, parameters );
    }
}
