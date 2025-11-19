using Microsoft.AspNetCore.Mvc;
using MindCabinet.Client.Services;
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


    [HttpPost(ClientDbAccess.SimplePost_GetByCriteria_Route)]
    public async Task<IEnumerable<SimplePostObject>> GetByCriteria_Async(
                ClientDbAccess.GetPostsByCriteriaParams parameters ) {
        using IDbConnection dbCon = await this.DbAccess.ConnectDb_Async();

        return await this.DbAccess.GetPostsByCriteria_Async( dbCon, parameters );
    }

    [HttpPost(ClientDbAccess.SimplePost_GetCountByCriteria_Route)]
    public async Task<int> GetCountByCriteria_Async(
                ClientDbAccess.GetPostsByCriteriaParams parameters ) {
        using IDbConnection dbCon = await this.DbAccess.ConnectDb_Async();

        return await this.DbAccess.GetPostCountByCriteria_Async( dbCon, parameters );
    }

    [HttpPost(ClientDbAccess.SimplePost_Create_Route)]
    public async Task<SimplePostObject> Create_Async( ClientDbAccess.CreatePostParams parameters ) {
        using IDbConnection dbCon = await this.DbAccess.ConnectDb_Async();

        return await this.DbAccess.CreatePost_Async( dbCon, parameters, this.SessData, false );
    }
}
