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

namespace MindCabinet.Hubs;


[HubRoute( ClientDataAccess_UserTermFavorites.IAPI.BaseRoute )]
public partial class UserTermFavoritesController : Hub, ClientDataAccess_UserTermFavorites.IAPI {
    private readonly DbAccess DbAccess;

    private readonly IServiceProvider ServiceProvider;

    private readonly ServerDataAccess_UserTermFavorites FavoriteTermsData;

    private readonly ClientSessionManager SessionManager;



    public UserTermFavoritesController(
                DbAccess dbAccess,
                IServiceProvider serviceProvider,
                ServerDataAccess_UserTermFavorites favoriteTermsData,
				ClientSessionManager sessMngr ) {
        this.DbAccess = dbAccess;
        this.ServiceProvider = serviceProvider;
        this.FavoriteTermsData = favoriteTermsData;
        this.SessionManager = sessMngr;
    }

    
    public async Task<IEnumerable<UserTermFavoriteObject.Raw>> GetFavTermsForCurrentUser_Async( object _ ) {
        if( !this.SessionManager.IsLoaded ) {
            HttpContext? context = this.Context.GetHttpContext();
            if( context is null ) {
                throw new InvalidOperationException( $"No HttpContext in {this.GetType().Name}" );
            }
            await ClientSessionManager.LoadForHubRequest_Async( this.ServiceProvider );
        }

        if( this.SessionManager.UserOfSession is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        return await this.FavoriteTermsData
            .GetFavTermEntriesBySimpleUserId_Async( dbCon, this.SessionManager.UserOfSession.Id );
    }


    public async Task AddTermsForCurrentUser_Async(
                ClientDataAccess_UserTermFavorites.IAPI.AddTermsForCurrentUser_Params parameters ) {
        if( !this.SessionManager.IsLoaded ) {
            HttpContext? context = this.Context.GetHttpContext();
            if( context is null ) {
                throw new InvalidOperationException( $"No HttpContext in {this.GetType().Name}" );
            }
            await ClientSessionManager.LoadForHubRequest_Async( this.ServiceProvider );
        }

        if( this.SessionManager.UserOfSession is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        await this.FavoriteTermsData.AddFavTermEntries_Async( dbCon, this.SessionManager.UserOfSession.Id, parameters.TermIds );
    }


    public async Task RemoveTermsForCurrentUser_Async(
                ClientDataAccess_UserTermFavorites.IAPI.RemoveTermsForCurrentUser_Params parameters ) {
        if( !this.SessionManager.IsLoaded ) {
            HttpContext? context = this.Context.GetHttpContext();
            if( context is null ) {
                throw new InvalidOperationException( $"No HttpContext in {this.GetType().Name}" );
            }
            await ClientSessionManager.LoadForHubRequest_Async( this.ServiceProvider );
        }

        if( this.SessionManager.UserOfSession is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        await this.FavoriteTermsData.RemoveFavTermEntries_Async( dbCon, this.SessionManager.UserOfSession.Id, parameters );
    }
}
