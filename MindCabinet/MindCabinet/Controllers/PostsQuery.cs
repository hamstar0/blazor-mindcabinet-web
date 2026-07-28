using Microsoft.AspNetCore.Mvc;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Data;
using MindCabinet.Data.DataAccess;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsQuery;
using System.Data;
using System.Text.Json;
using MindCabinet.Services;
using MindCabinet.Utility.Attributes;
using Microsoft.AspNetCore.SignalR;


namespace MindCabinet.Controllers;


// [HubRoute( ClientDataAccess_PostsQuery.IAPI.BaseRoute )]
// [Route("[controller]")]
[ApiController]
[Route( ClientDataAccess_PostsQuery.IAPI.BaseRoute )]
public class PostsQueryController(
                ILogger<PostsQueryController> logger,
                IServiceProvider serviceProvider,
                DbAccess dbAccess,
                ServerDataAccess_PostsQuery postsQueryDataSrc,
                ServerDataAccess_PostsQueryTermEntry postsQueryTermEntryDataSrc,
				ClientSessionManager sessMngr
            ) : ControllerBase, ClientDataAccess_PostsQuery.IAPI {
    private readonly ILogger<PostsQueryController> Logger = logger;

    private readonly IServiceProvider ServiceProvider = serviceProvider;

    private readonly DbAccess DbAccess = dbAccess;

    private readonly ServerDataAccess_PostsQuery PostsQueryDataSrc = postsQueryDataSrc;

    private readonly ServerDataAccess_PostsQueryTermEntry PostsQueryTermEntryDataSrc = postsQueryTermEntryDataSrc;

    private readonly ClientSessionManager SessionManager = sessMngr;



    [HttpPost(nameof(GetForCurrentUserByCriteria_Async))]
    public async Task<ClientDataAccess_PostsQuery.IAPI.Get_Return> GetForCurrentUserByCriteria_Async(
                ClientDataAccess_PostsQuery.IAPI.GetByCriteria_Params parameters ) {
        if( this.SessionManager.UserOfSession is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        IEnumerable<PostsQueryObject.Raw> queries = await this.PostsQueryDataSrc.GetByCriteria_Async(
            dbCon: dbCon,
            postsQueryTermEntryDataSrc: this.PostsQueryTermEntryDataSrc,
            parameters: parameters,
            owner: this.SessionManager.UserOfSession.Id,
            alsoGetEntries: true
        );

        return new ClientDataAccess_PostsQuery.IAPI.Get_Return { Queries = queries };
    }


    [HttpPost(nameof(CreateForCurrentUser_Async))]
    public async Task<ClientDataAccess_PostsQuery.IAPI.CreateOrUpdate_Return> CreateForCurrentUser_Async(
                PostsQueryObject.Prototype parameters ) {
        if( this.SessionManager.UserOfSession is null ) {
            throw new InvalidOperationException( "No user in session" );
        }
        if( !parameters.IsValid(false) ) {
            throw new ArgumentException( "Invalid PostsQueryObject.Prototype in parameters." );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        return await this.PostsQueryDataSrc.Create_Async(
            dbCon: dbCon,
            postsQueryTermEntryDataSrc: this.PostsQueryTermEntryDataSrc,
            parameters: parameters,
            owner: this.SessionManager.UserOfSession.Id
        );
    }

    [HttpPost(nameof(UpdateForCurrentUser_Async))]
    public async Task<ClientDataAccess_PostsQuery.IAPI.CreateOrUpdate_Return> UpdateForCurrentUser_Async(
                PostsQueryObject.Prototype parameters ) {
        if( this.SessionManager.UserOfSession is null ) {
            throw new InvalidOperationException( "No user in session" );
        }
        if( parameters.Id == 0 ) {
            throw new ArgumentException( "PostsQueryObject.Prototype Id is not valid (must be non-null and non-zero)." );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        parameters.Owner = this.SessionManager.UserOfSession.Id;

        ClientDataAccess_PostsQuery.IAPI.CreateOrUpdate_Return ret = await this.PostsQueryDataSrc.Update_Async(
            dbCon: dbCon,
            postsQueryTermEntryDataSrc: this.PostsQueryTermEntryDataSrc,
            parameters: parameters
        );

        return ret;
    }
}
