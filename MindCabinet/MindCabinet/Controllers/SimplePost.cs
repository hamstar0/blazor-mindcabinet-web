using Microsoft.AspNetCore.Mvc;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Data;
using MindCabinet.Data.DataAccess;
using MindCabinet.Shared.DataObjects;
using System.Data;


namespace MindCabinet.Controllers;


[ApiController]
[Route("[controller]")]
public class SimplePostController : ControllerBase {
    private readonly DbAccess DbAccess;

    private readonly ServerDataAccess_SimplePosts SimplePostsData;

    private readonly ServerDataAccess_Terms TermsData;

    private readonly ServerDataAccess_Terms_Sets TermSetsData;

    private readonly ServerDataAccess_UserTermsHistory UserTermsHistoryData;

    private readonly ServerSessionData SessionData;



    public SimplePostController(
                DbAccess dbAccess,
                ServerDataAccess_SimplePosts simplePostsData,
                ServerDataAccess_Terms termsData,
                ServerDataAccess_Terms_Sets termSetsData,
                ServerDataAccess_UserTermsHistory userTermsHistoryData,
                ServerSessionData sessData ) {
        //this.HttpContext
        this.DbAccess = dbAccess;
        this.SimplePostsData = simplePostsData;
        this.TermsData = termsData;
        this.TermSetsData = termSetsData;
        this.UserTermsHistoryData = userTermsHistoryData;
        this.SessionData = sessData;
    }


    [HttpPost(ClientDataAccess_SimplePosts.GetByCriteria_Route)]
    public async Task<IEnumerable<SimplePostObject>> GetByCriteria_Async(
                ClientDataAccess_SimplePosts.GetByCriteria_Params parameters ) {
        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async();

        return await this.SimplePostsData.GetByCriteria_Async( dbCon, this.TermsData, this.TermSetsData, parameters );
    }

    [HttpPost(ClientDataAccess_SimplePosts.GetCountByCriteria_Route)]
    public async Task<int> GetCountByCriteria_Async(
                ClientDataAccess_SimplePosts.GetByCriteria_Params parameters ) {
        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async();

        return await this.SimplePostsData.GetCountByCriteria_Async( dbCon, parameters );
    }

    [HttpPost(ClientDataAccess_SimplePosts.Create_Route)]
    public async Task<SimplePostObject> Create_Async( ClientDataAccess_SimplePosts.Create_Params parameters ) {
        if( this.SessionData.User is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async();

        return await this.SimplePostsData.Create_Async(
            dbCon: dbCon,
            simpleUserId: this.SessionData.User.Id,
            termSetsData: this.TermSetsData,
            termHistoryData: this.UserTermsHistoryData,
            parameters: parameters,
            skipHistory: false
        );
    }
}
