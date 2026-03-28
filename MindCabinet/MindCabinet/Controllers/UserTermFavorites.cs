using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Data;
using MindCabinet.Data.DataAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.UserTermFavorite;
using MindCabinet.Shared.DataObjects.UserTermHistory;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace MindCabinet.Controllers;


[ApiController]
[Route("[controller]")]
public partial class UserTermFavoritesController : ControllerBase {
    private readonly DbAccess DbAccess;

    private readonly ServerDataAccess_UserTermFavorites FavoriteTermsData;

    private readonly ServerSessionData SessionData;



    public UserTermFavoritesController(
                DbAccess dbAccess,
                ServerDataAccess_SimpleUsers simpleUsersData,
                ServerDataAccess_SimpleUserSessions sessionsData,
                ServerDataAccess_UserTermFavorites favoriteTermsData,
                ServerSessionData sessionData ) {
        this.DbAccess = dbAccess;
        this.FavoriteTermsData = favoriteTermsData;
        this.SessionData = sessionData;
    }

    
    [HttpPost(ClientDataAccess_UserTermFavorites.GetFavTermsForCurrentUser_Route)]
    public async Task<IEnumerable<UserTermFavoriteObject.Raw>> GetTermIdsForCurrentUserId_Async(
                ClientDataAccess_UserTermFavorites.GetTermIdsForCurrentUser_Params parameters ) {
        if( this.SessionData.UserOfSession is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        return await this.FavoriteTermsData
            .GetFavTermEntries_Async( dbCon, this.SessionData.UserOfSession.Id, parameters );
    }


    [HttpPost(ClientDataAccess_UserTermFavorites.AddTermsForCurrentUser_Route)]
    public async Task AddTermIdsForCurrentUser_Async(
                ClientDataAccess_UserTermFavorites.AddTermsForCurrentUser_Params parameters ) {
        if( this.SessionData.UserOfSession is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        await this.FavoriteTermsData.AddFavTermEntries_Async( dbCon, this.SessionData.UserOfSession.Id, parameters.TermIds );
    }


    [HttpPost(ClientDataAccess_UserTermFavorites.RemoveTermsForCurrentUser_Route)]
    public async Task RemoveTermIdsForCurrentUser_Async(
                ClientDataAccess_UserTermFavorites.RemoveTermsForCurrentUser_Params parameters ) {
        if( this.SessionData.UserOfSession is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        await this.FavoriteTermsData.RemoveFavTermEntries_Async( dbCon, this.SessionData.UserOfSession.Id, parameters );
    }
}
