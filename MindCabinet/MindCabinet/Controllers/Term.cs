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


    [HttpPost(nameof(ClientDbAccess.Term_GetByCriteria_Route))]
    public async Task<IEnumerable<TermObject>> GetByCriteria_Async(
                ClientDbAccess.GetTermsByCriteriaParams parameters ) {
        using IDbConnection dbCon = await this.DbAccess.ConnectDb_Async();

        return await this.DbAccess.GetTermsByCriteria_Async( dbCon, parameters );
    }

    [HttpPost(ClientDbAccess.Term_Create_Route)]
    public async Task<ClientDbAccess.CreateTermReturn> Create_Async( ClientDbAccess.CreateTermParams parameters ) {
        using IDbConnection dbCon = await this.DbAccess.ConnectDb_Async();

        return await this.DbAccess.CreateTerm_Async( dbCon, parameters );
    }
}
