using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Data;
using MindCabinet.Data.DataAccess;
using MindCabinet.Services;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.UserTermFavorite;
using MindCabinet.Shared.DataObjects.UserTermHistory;
using MindCabinet.Utility.Attributes;
using System.Data;
using System.Security.Cryptography;
using System.Text;


namespace MindCabinet.Controllers;


// [HubRoute( ClientDataAccess_UserTermFavorites.IAPI.BaseRoute )]
// [Route("[controller]")]
[ApiController]
[Route( ClientDataAccess_UserTermFavorites.IAPI.BaseRoute )]
public partial class UserTermFavoritesController : ControllerBase, ClientDataAccess_UserTermFavorites.IAPI {
    private readonly DbAccess DbAccess;

    private readonly IServiceProvider ServiceProvider;

    private readonly ServerDataAccess_UserTermFavorites FavoriteTermsDataSrc;

    private readonly ClientSessionManager SessionManager;



    public UserTermFavoritesController(
                DbAccess dbAccess,
                IServiceProvider serviceProvider,
                ServerDataAccess_UserTermFavorites favoriteTermsDataSrc,
				ClientSessionManager sessMngr ) {
        this.DbAccess = dbAccess;
        this.ServiceProvider = serviceProvider;
        this.FavoriteTermsDataSrc = favoriteTermsDataSrc;
        this.SessionManager = sessMngr;
    }

    
    [HttpPost(nameof(GetFavTermsForCurrentUser_Async))]
    public async Task<IEnumerable<UserTermFavoriteObject.Raw>> GetFavTermsForCurrentUser_Async( object _ ) {
        if( this.SessionManager.UserOfSession is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        return await this.FavoriteTermsDataSrc
            .GetFavTermEntriesBySimpleUserId_Async( dbCon, this.SessionManager.UserOfSession.Id );
    }


    [HttpPost(nameof(AddTermsForCurrentUser_Async))]
    public async Task<object> AddTermsForCurrentUser_Async(
                ClientDataAccess_UserTermFavorites.IAPI.EditForCurrentUser_Params parameters ) {
        if( this.SessionManager.UserOfSession is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        await this.FavoriteTermsDataSrc.AddFavTermEntries_Async(
            dbCon,
            this.SessionManager.UserOfSession.Id,
            parameters
        );

        return new object();
    }


    [HttpPost(nameof(RemoveTermsForCurrentUser_Async))]
    public async Task<object> RemoveTermsForCurrentUser_Async(
                ClientDataAccess_UserTermFavorites.IAPI.EditForCurrentUser_Params parameters ) {
        if( this.SessionManager.UserOfSession is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        await this.FavoriteTermsDataSrc.RemoveFavTermEntries_Async(
            dbCon,
            this.SessionManager.UserOfSession.Id,
            parameters
        );

        return new object();
    }


    [HttpPost(nameof(UpdateTermsForCurrentUser_Async))]
    public async Task<object> UpdateTermsForCurrentUser_Async(
                ClientDataAccess_UserTermFavorites.IAPI.EditForCurrentUser_Params parameters ) {
        if( this.SessionManager.UserOfSession is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        await this.FavoriteTermsDataSrc.UpdateFavTermEntries_Async(
            dbCon,
            this.SessionManager.UserOfSession.Id,
            parameters
        );

        return new object();
    }
}
