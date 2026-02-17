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


namespace MindCabinet.Controllers;


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

    
    [HttpPost(ClientDataAccess_UserTermsHistory.GetTermIdsForCurrentUser_Route)]
    public async Task<IEnumerable<ClientDataAccess_UserTermsHistory.GetTermIdsForCurrentUser_Return>> GetForCurrentUserId_Async(
                ClientDataAccess_UserTermsHistory.GetTermIdsForCurrentUser_Params parameters ) {
        if( this.SessionData.UserOfSession is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        return await this.UserTermsHistoryData.GetByUserId_Async( dbCon, this.SessionData.UserOfSession.Id, parameters );
    }


    [HttpPost(ClientDataAccess_UserTermsHistory.AddTermsForCurrentUser_Route)]
    public async Task AddFavoriteTermsByIdForCurrentUserId_Async(
                ClientDataAccess_UserTermsHistory.AddTermsForCurrentUser_Params parameters ) {
        if( this.SessionData.UserOfSession is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        await this.UserTermsHistoryData.AddTerm_Async( dbCon, this.SessionData.UserOfSession.Id, parameters );
    }
}
