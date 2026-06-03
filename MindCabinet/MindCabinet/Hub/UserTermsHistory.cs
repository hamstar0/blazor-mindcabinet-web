using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Data;
using MindCabinet.Data.DataAccess;
using MindCabinet.Services;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.UserTermHistory;
using MindCabinet.Utility.Attributes;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace MindCabinet.Hubs;


[HubRoute( ClientDataAccess_UserTermsHistory.IAPI.BaseRoute )]
public partial class UserTermsHistoryController : Hub, ClientDataAccess_UserTermsHistory.IAPI {
    private readonly DbAccess DbAccess;

    private readonly IServiceProvider ServiceProvider;

    private readonly ServerDataAccess_UserTermsHistory UserTermsHistoryDataSrc;

    private readonly ClientSessionManager SessionManager;



    public UserTermsHistoryController(
                DbAccess dbAccess,
                IServiceProvider serviceProvider,
                ServerDataAccess_UserTermsHistory userTermsHistoryDataSrc,
				ClientSessionManager sessionData ) {
        this.DbAccess = dbAccess;
        this.ServiceProvider = serviceProvider;
        this.UserTermsHistoryDataSrc = userTermsHistoryDataSrc;
        this.SessionManager = sessionData;
    }

    
    public async Task<IEnumerable<UserTermHistoryObject.Raw>> GetHistTermsForCurrentUser_Async() {
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

        return await this.UserTermsHistoryDataSrc.GetByUserId_Async( dbCon, this.SessionManager.UserOfSession.Id );
    }


    public async Task AddHistTermsForCurrentUser_Async(
                ClientDataAccess_UserTermsHistory.IAPI.AddHistTermsForCurrentUser_Params parameters ) {
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

        await this.UserTermsHistoryDataSrc.AddTerm_Async( dbCon, this.SessionManager.UserOfSession.Id, parameters );
    }
}
