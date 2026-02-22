using Microsoft.AspNetCore.Mvc;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Client.Services.DbAccess.Joined;
using MindCabinet.Data;
using MindCabinet.Data.DataAccess;
using MindCabinet.Data.DataAccess.Composite;
using MindCabinet.Shared.DataObjects;
using System.Data;
using static MindCabinet.Data.DataAccess.Composite.ServerDataAccess_PrioritizedPosts;


namespace MindCabinet.Controllers.Composite;


[ApiController]
[Route("[controller]")]
public class PrioritizedPostController : ControllerBase {
    private readonly DbAccess DbAccess;

    private readonly ServerDataAccess_PrioritizedPosts PrioritizedPostsData;

    private readonly ServerDataAccess_Terms TermsData;

    private readonly ServerDataAccess_TermSets TermSetsData;

    private readonly ServerDataAccess_UserTermsHistory UserTermsHistoryData;

    private readonly ServerDataAccess_UserContexts UserContextData;

    private readonly ServerSessionData SessionData;



    public PrioritizedPostController(
                DbAccess dbAccess,
                ServerDataAccess_PrioritizedPosts prioritizedPostsData,
                ServerDataAccess_Terms termsData,
                ServerDataAccess_TermSets termSetsData,
                ServerDataAccess_UserTermsHistory userTermsHistoryData,
                ServerDataAccess_UserContexts userContextData,
                ServerSessionData sessData ) {
        //this.HttpContext
        this.DbAccess = dbAccess;
        this.PrioritizedPostsData = prioritizedPostsData;
        this.TermsData = termsData;
        this.TermSetsData = termSetsData;
        this.UserTermsHistoryData = userTermsHistoryData;
        this.UserContextData = userContextData;
        this.SessionData = sessData;
    }


    [HttpPost(ClientDataAccess_PrioritizedPosts.GetByCriteria_Route)]
    public async Task<IEnumerable<SimplePostObject.DatabaseEntry>> GetByCriteria_Async(
                ClientDataAccess_PrioritizedPosts.GetByCriteria_Params parameters ) {
        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        return await this.PrioritizedPostsData.GetByCriteria_Async(
            dbCon: dbCon,
            termsData: this.TermsData,
            termSetsData: this.TermSetsData,
            userContextData: this.UserContextData,
            parameters: parameters
        );
    }

    [HttpPost(ClientDataAccess_PrioritizedPosts.GetCountByCriteria_Route)]
    public async Task<int> GetCountByCriteria_Async(
                ClientDataAccess_PrioritizedPosts.GetByCriteria_Params parameters ) {
        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        return await this.PrioritizedPostsData.GetCountByCriteria_Async(
            dbCon: dbCon,
            termsData: this.TermsData,
            termSetsData: this.TermSetsData,
            userContextData: this.UserContextData,
            parameters: parameters
        );
    }
}
