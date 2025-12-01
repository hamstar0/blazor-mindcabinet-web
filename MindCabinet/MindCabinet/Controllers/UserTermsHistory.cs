using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Data;
using MindCabinet.Data.DataAccess;
using MindCabinet.Shared.DataObjects;
using System.Data;
using System.Security.Cryptography;
using System.Text;


namespace MindCabinet;


[ApiController]
[Route("[controller]")]
public partial class UserTermsHistoryController : ControllerBase {
    private readonly DbAccess DbAccess;

    private readonly ServerDataAccess_UserTermsHistory UserTermsHistoryData;

    private readonly ServerSessionData SessionData;



    public UserTermsHistoryController(
                DbAccess dbAccess,
                ServerDataAccess_UserTermsHistory userTermsHistoryData,
                ServerSessionData sessionData ) {
        this.DbAccess = dbAccess;
        this.UserTermsHistoryData = userTermsHistoryData;
        this.SessionData = sessionData;
    }

    
    [HttpPost(ClientDataAccess_UserTermsHistory.GetByUserId_Route)]
    public async Task<IEnumerable<ClientDataAccess_UserTermsHistory.GetByUserId_Return>> GetByUserId_Async(
                ClientDataAccess_UserTermsHistory.GetByUserId_Params parameters ) {
        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async();

        return await this.UserTermsHistoryData.GetByUserId_Async( dbCon, parameters );
    }


    [HttpPost(ClientDataAccess_UserTermsHistory.Add_Route)]
    public async Task AddFavoriteTermsById_Async(
                ClientDataAccess_UserTermsHistory.Add_Params parameters ) {
        if( this.SessionData.User is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async();

        await this.UserTermsHistoryData.AddTerm_Async( dbCon, this.SessionData.User.Id, parameters );
    }
}
