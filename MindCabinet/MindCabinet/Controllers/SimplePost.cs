using Microsoft.AspNetCore.Mvc;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Data;
using MindCabinet.Data.DataAccess;
using MindCabinet.Shared.DataObjects;
using System.Data;


namespace MindCabinet;


[ApiController]
[Route("[controller]")]
public class SimplePostController : ControllerBase {
    private readonly DbAccess DbAccess;

    private readonly ServerDataAccess_SimplePosts SimplePostsData;

    private readonly ServerDataAccess_Terms TermsData;

    private readonly ServerDataAccess_Terms_Sets TermSetsData;

    private readonly ServerSessionData SessData;



    public SimplePostController(
                DbAccess dbAccess,
                ServerDataAccess_SimplePosts simplePostsData,
                ServerDataAccess_Terms termsData,
                ServerDataAccess_Terms_Sets termSetsData,
                ServerSessionData sessData ) {
        //this.HttpContext
        this.DbAccess = dbAccess;
        this.SimplePostsData = simplePostsData;
        this.TermsData = termsData;
        this.TermSetsData = termSetsData;
        this.SessData = sessData;
    }


    [HttpPost(ClientDataAccess_SimplePosts.GetByCriteria_Route)]
    public async Task<IEnumerable<SimplePostObject>> GetByCriteria_Async(
                ClientDataAccess_SimplePosts.GetByCriteria_Params parameters ) {
        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async();

        return await this.SimplePostsData.GetSimplePostsByCriteria_Async( dbCon, this.TermsData, this.TermSetsData, parameters );
    }

    [HttpPost(ClientDataAccess_SimplePosts.GetCountByCriteria_Route)]
    public async Task<int> GetCountByCriteria_Async(
                ClientDataAccess_SimplePosts.GetByCriteria_Params parameters ) {
        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async();

        return await this.SimplePostsData.GetSimplePostCountByCriteria_Async( dbCon, parameters );
    }

    [HttpPost(ClientDataAccess_SimplePosts.Create_Route)]
    public async Task<SimplePostObject> Create_Async( ClientDataAccess_SimplePosts.Create_Params parameters ) {
        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async();

        return await this.SimplePostsData.CreateSimplePost_Async( dbCon, this.TermSetsData, parameters, this.SessData, false );
    }
}
