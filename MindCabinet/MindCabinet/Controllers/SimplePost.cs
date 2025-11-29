using Microsoft.AspNetCore.Mvc;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Data;
using MindCabinet.Shared.DataObjects;
using System.Data;


namespace MindCabinet;


[ApiController]
[Route("[controller]")]
public class SimplePostController : ControllerBase {
    private readonly ServerDbAccess DbAccess;

    private readonly ServerSessionData SessData;



    public SimplePostController( ServerDbAccess dbAccess, ServerSessionData sessData ) {
        //this.HttpContext
        this.DbAccess = dbAccess;
        this.SessData = sessData;
    }


    [HttpPost(ClientDbAccess_SimplePosts.GetByCriteria_Route)]
    public async Task<IEnumerable<SimplePostObject>> GetByCriteria_Async(
                ClientDbAccess_SimplePosts.GetByCriteria_Params parameters ) {
        using IDbConnection dbCon = await this.DbAccess.ConnectDb_Async();

        return await this.DbAccess.GetSimplePostsByCriteria_Async( dbCon, parameters );
    }

    [HttpPost(ClientDbAccess_SimplePosts.GetCountByCriteria_Route)]
    public async Task<int> GetCountByCriteria_Async(
                ClientDbAccess_SimplePosts.GetByCriteria_Params parameters ) {
        using IDbConnection dbCon = await this.DbAccess.ConnectDb_Async();

        return await this.DbAccess.GetSimplePostCountByCriteria_Async( dbCon, parameters );
    }

    [HttpPost(ClientDbAccess_SimplePosts.Create_Route)]
    public async Task<SimplePostObject> Create_Async( ClientDbAccess_SimplePosts.Create_Params parameters ) {
        using IDbConnection dbCon = await this.DbAccess.ConnectDb_Async();

        return await this.DbAccess.CreateSimplePost_Async( dbCon, parameters, this.SessData, false );
    }
}
