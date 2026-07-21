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


namespace MindCabinet.Controllers;


// [HubRoute( ClientDataAccess_Terms.IAPI.BaseRoute )]
// [Route("[controller]")]
[ApiController]
[Route( ClientDataAccess_Terms.IAPI.BaseRoute )]
public class TermController : ControllerBase, ClientDataAccess_Terms.IAPI {
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


    [HttpPost(nameof(GetByCriteria_Async))]
    public async Task<ClientDataAccess_Terms.IAPI.GetByX_Return> GetByCriteria_Async(
                ClientDataAccess_Terms.IAPI.GetByCriteria_Params parameters ) {
        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        IEnumerable<TermObject.Raw> terms =  await this.TermsDataSrc.GetTermsByCriteria_Async( dbCon, parameters );

        return new ClientDataAccess_Terms.IAPI.GetByX_Return( terms );
    }


    [HttpPost(nameof(GetByIds_Async))]
    public async Task<ClientDataAccess_Terms.IAPI.GetByX_Return> GetByIds_Async(
                IEnumerable<TermId> ids ) {
        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        IEnumerable<TermObject.Raw> terms = await this.TermsDataSrc.GetByIds_Async( dbCon, ids );

        return new ClientDataAccess_Terms.IAPI.GetByX_Return( terms );
    }


    [HttpPost(nameof(CreateForCurrentUser_Async))]
    public async Task<ClientDataAccess_Terms.IAPI.CreateForCurrentUser_Return> CreateForCurrentUser_Async(
                ClientDataAccess_Terms.IAPI.CreateForCurrentUser_Params parameters ) {
        if( this.SessionManager.UserOfSession is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        return await this.TermsDataSrc.Create_Async(
            dbCon,
            this.SessionManager.UserOfSession.Id,
            parameters
        );
    }


    [HttpPost(nameof(UpdateForCurrentUser_Async))]
    public async Task<bool> UpdateForCurrentUser_Async(
                ClientDataAccess_Terms.IAPI.UpdateForCurrentUser_Params parameters ) {
        if( this.SessionManager.UserOfSession is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        var term = await this.TermsDataSrc.GetById_Async( dbCon, parameters.Id );

        if( term is null ) {
            return false;   // leet haxor again
        }
        if( term.Creator != this.SessionManager.UserOfSession.Id ) {
            return false;   // leet haxor again
        }

        return await this.TermsDataSrc.Update_Async(
            dbCon,
            this.SessionManager.UserOfSession.Id,
            parameters
        );
    }
}
