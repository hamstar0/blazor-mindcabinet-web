using Microsoft.AspNetCore.Mvc;
using MindCabinet.Client.Services;
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


    [HttpPost(nameof(ClientDbAccess.Route_Term_GetByCriteria.route))]
    public async Task<IEnumerable<TermObject>> GetByCriteria_Async(
                ClientDbAccess.GetTermsByCriteriaParams parameters ) {
        using IDbConnection dbCon = await this.DbAccess.ConnectDb_Async();

        return await this.DbAccess.GetTermsByCriteria_Async( dbCon, parameters );
    }

    [HttpPost(nameof(ClientDbAccess.Route_Term_Create.route))]
    public async Task<ClientDbAccess.CreateTermReturn> Create_Async( ClientDbAccess.CreateTermParams parameters ) {
        using IDbConnection dbCon = await this.DbAccess.ConnectDb_Async();

        return await this.DbAccess.CreateTerm_Async( dbCon, parameters );
    }
}
