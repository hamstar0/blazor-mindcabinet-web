using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Data;
using MindCabinet.Data.DataAccess;
using MindCabinet.Services;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Utility.Attributes;
using System.Data;


namespace MindCabinet.Hubs;


[HubRoute( ClientDataAccess_Terms.IAPI.BaseRoute )]
public class TermController : Hub, ClientDataAccess_Terms.IAPI {
    private readonly DbAccess DbAccess;
    
    private readonly IServiceProvider ServiceProvider;

    private readonly ServerDataAccess_Terms TermsDataSrc;

    private readonly ClientSessionManager SessionManager;



    public TermController(
                DbAccess dbAccess,
                IServiceProvider serviceProvider,
                ServerDataAccess_Terms termsDataSrc,
                ClientSessionManager sessionManager ) {
        this.DbAccess = dbAccess;
        this.ServiceProvider = serviceProvider;
        this.TermsDataSrc = termsDataSrc;
        this.SessionManager = sessionManager;
    }


    public async Task<ClientDataAccess_Terms.IAPI.GetByX_Return> GetByCriteria_Async(
                ClientDataAccess_Terms.IAPI.GetByCriteria_Params parameters ) {
        if( !this.SessionManager.IsLoaded ) {
            HttpContext? context = this.Context.GetHttpContext();
            if( context is null ) {
                throw new InvalidOperationException( $"No HttpContext in {this.GetType().Name}" );
            }
            await ClientSessionManager.LoadForHubRequest_Async( this.ServiceProvider );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        IEnumerable<TermObject.Raw> terms =  await this.TermsDataSrc.GetTermsByCriteria_Async( dbCon, parameters );

        return new ClientDataAccess_Terms.IAPI.GetByX_Return( terms );
    }


    public async Task<ClientDataAccess_Terms.IAPI.GetByX_Return> GetByIds_Async(
                IEnumerable<TermId> ids ) {
        if( !this.SessionManager.IsLoaded ) {
            HttpContext? context = this.Context.GetHttpContext();
            if( context is null ) {
                throw new InvalidOperationException( $"No HttpContext in {this.GetType().Name}" );
            }
            await ClientSessionManager.LoadForHubRequest_Async( this.ServiceProvider );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        IEnumerable<TermObject.Raw> terms = await this.TermsDataSrc.GetByIds_Async( dbCon, ids );

        return new ClientDataAccess_Terms.IAPI.GetByX_Return( terms );
    }


    public async Task<ClientDataAccess_Terms.IAPI.Create_Return> Create_Async(
                ClientDataAccess_Terms.IAPI.Create_Params parameters ) {
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

        return await this.TermsDataSrc.Create_Async( dbCon, parameters );
    }
}
