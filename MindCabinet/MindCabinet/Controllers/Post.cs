using Microsoft.AspNetCore.Mvc;
using MindCabinet.Client.Services;
using MindCabinet.Data;
using MindCabinet.Shared.DataEntries;
using System.Data;


namespace MindCabinet;


[ApiController]
[Route("[controller]")]
public class PostController : ControllerBase {
    private readonly ServerDbAccess DbAccess;



    public PostController( ServerDbAccess dbAccess ) {
        //this.HttpContext
        this.DbAccess = dbAccess;
    }


    [HttpPost("GetByCriteria")]
    public async Task<IEnumerable<PostEntry>> GetByCriteria_Async(
                ClientDbAccess.GetPostsByCriteriaParams parameters ) {
        using IDbConnection dbCon = await this.DbAccess.ConnectDb_Async();

        return await this.DbAccess.GetPostsByCriteria_Async( dbCon, parameters );
    }

    [HttpPost("GetCountByCriteria")]
    public async Task<int> GetCountByCriteria_Async(
                ClientDbAccess.GetPostsByCriteriaParams parameters ) {
        using IDbConnection dbCon = await this.DbAccess.ConnectDb_Async();

        return await this.DbAccess.GetPostCountByCriteria_Async( dbCon, parameters );
    }

    [HttpPost("Create")]
    public async Task<PostEntry> Create_Async( ClientDbAccess.CreatePostParams parameters ) {
        using IDbConnection dbCon = await this.DbAccess.ConnectDb_Async();

        return await this.DbAccess.CreatePost_Async( dbCon, parameters );
    }
}
