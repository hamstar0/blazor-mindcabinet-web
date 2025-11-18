using Microsoft.AspNetCore.Mvc;
using MindCabinet.Client.Services;
using MindCabinet.Data;
using MindCabinet.Shared.DataObjects;
using System.Data;


namespace MindCabinet;


[ApiController]
[Route("[controller]")]
public class PostController : ControllerBase {
    private readonly ServerDbAccess DbAccess;

    private readonly ServerSessionData SessData;



    public PostController( ServerDbAccess dbAccess, ServerSessionData sessData ) {
        //this.HttpContext
        this.DbAccess = dbAccess;
        this.SessData = sessData;
    }


    [HttpPost(ClientDbAccess.Post_GetByCriteria_Route)]
    public async Task<IEnumerable<PostObject>> GetByCriteria_Async(
                ClientDbAccess.GetPostsByCriteriaParams parameters ) {
        using IDbConnection dbCon = await this.DbAccess.ConnectDb_Async();

        return await this.DbAccess.GetPostsByCriteria_Async( dbCon, parameters );
    }

    [HttpPost(ClientDbAccess.Post_GetCountByCriteria_Route)]
    public async Task<int> GetCountByCriteria_Async(
                ClientDbAccess.GetPostsByCriteriaParams parameters ) {
        using IDbConnection dbCon = await this.DbAccess.ConnectDb_Async();

        return await this.DbAccess.GetPostCountByCriteria_Async( dbCon, parameters );
    }

    [HttpPost(ClientDbAccess.Post_Create_Route)]
    public async Task<PostObject> Create_Async( ClientDbAccess.CreatePostParams parameters ) {
        using IDbConnection dbCon = await this.DbAccess.ConnectDb_Async();

        return await this.DbAccess.CreatePost_Async( dbCon, parameters, this.SessData, false );
    }
}
