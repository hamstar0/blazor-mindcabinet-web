using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Data;
using MindCabinet.Data.DataAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.UserFavoriteTerm;
using System.Data;
using System.Security.Cryptography;
using System.Text;


namespace MindCabinet.Controllers;


[ApiController]
[Route("[controller]")]
public partial class UserFavoriteTermsController : ControllerBase {
    private readonly DbAccess DbAccess;

    private readonly ServerDataAccess_UserFavoriteTerms FavoriteTermsData;

    private readonly ServerSessionData SessionData;



    public UserFavoriteTermsController(
                DbAccess dbAccess,
                ServerDataAccess_SimpleUsers simpleUsersData,
                ServerDataAccess_SimpleUserSessions sessionsData,
                ServerDataAccess_UserFavoriteTerms favoriteTermsData,
                ServerSessionData sessionData ) {
        this.DbAccess = dbAccess;
        this.FavoriteTermsData = favoriteTermsData;
        this.SessionData = sessionData;
    }

    
    [HttpPost(ClientDataAccess_UserFavoriteTerms.GetTermIdsForCurrentUser_Route)]
    public async Task<IEnumerable<long>> GetTermIdsForCurrentUserId_Async(
                ClientDataAccess_UserFavoriteTerms.GetTermIdsForCurrentUser_Params parameters ) {
        if( this.SessionData.UserOfSession is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        IEnumerable<UserFavoriteTermObject.DatabaseEntry> termsRaw = await this.FavoriteTermsData
            .GetFavTermEntries_Async( dbCon, this.SessionData.UserOfSession.Id, parameters );
        return termsRaw.Select( e => e.FavTermId );
    }


    [HttpPost(ClientDataAccess_UserFavoriteTerms.AddTermsForCurrentUser_Route)]
    public async Task AddTermIdsForCurrentUser_Async(
                ClientDataAccess_UserFavoriteTerms.AddTermsForCurrentUser_Params parameters ) {
        if( this.SessionData.UserOfSession is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        await this.FavoriteTermsData.AddFavTermEntries_Async( dbCon, this.SessionData.UserOfSession.Id, parameters.TermIds );
    }


    [HttpPost(ClientDataAccess_UserFavoriteTerms.RemoveTermsForCurrentUser_Route)]
    public async Task RemoveTermIdsForCurrentUser_Async(
                ClientDataAccess_UserFavoriteTerms.RemoveTermsForCurrentUser_Params parameters ) {
        if( this.SessionData.UserOfSession is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        await this.FavoriteTermsData.RemoveFavTermEntries_Async( dbCon, this.SessionData.UserOfSession.Id, parameters );
    }
}
