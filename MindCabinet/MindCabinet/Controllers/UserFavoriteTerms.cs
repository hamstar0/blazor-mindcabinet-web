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
public partial class UserFavoriteTermsController : ControllerBase {
    private readonly DbAccess DbAccess;

    private readonly ServerDataAccess_UserFavoriteTerms FavoriteTermsData;

    private readonly ServerSessionData SessionData;



    public UserFavoriteTermsController(
                DbAccess dbAccess,
                ServerDataAccess_SimpleUsers simpleUsersData,
                ServerDataAccess_SimpleUsers_Sessions sessionsData,
                ServerDataAccess_UserFavoriteTerms favoriteTermsData,
                ServerSessionData sessionData ) {
        this.DbAccess = dbAccess;
        this.FavoriteTermsData = favoriteTermsData;
        this.SessionData = sessionData;
    }

    
    [HttpPost(ClientDataAccess_UserFavoriteTerms.GetTermIdsForCurrentUser_Route)]
    public async Task<IEnumerable<long>> GetTermIdsForCurrentUserId_Async(
                ClientDataAccess_UserFavoriteTerms.GetTermIdsForCurrentUser_Params parameters ) {
        if( this.SessionData.User is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async();

        return await this.FavoriteTermsData.GetTermIds_Async( dbCon, this.SessionData.User.Id, parameters );
    }


    [HttpPost(ClientDataAccess_UserFavoriteTerms.AddTermsForCurrentUser_Route)]
    public async Task AddTermIdsForCurrentUser_Async(
                ClientDataAccess_UserFavoriteTerms.AddTermsForCurrentUser_Params parameters ) {
        if( this.SessionData.User is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async();

        await this.FavoriteTermsData.AddTermIds_Async( dbCon, this.SessionData.User.Id, parameters );
    }
}
