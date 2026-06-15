using Microsoft.AspNetCore.Mvc;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Data;
using MindCabinet.Data.DataAccess;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsContext;
using System.Data;
using System.Text.Json;
using MindCabinet.Services;
using MindCabinet.Utility.Attributes;
using Microsoft.AspNetCore.SignalR;


namespace MindCabinet.Controllers;


// [HubRoute( ClientDataAccess_PostsContext.IAPI.BaseRoute )]
// [Route("[controller]")]
[ApiController]
[Route( ClientDataAccess_PostsContext.IAPI.BaseRoute )]
public class PostsContextController(
                ILogger<PostsContextController> logger,
                IServiceProvider serviceProvider,
                DbAccess dbAccess,
                ServerDataAccess_PostsContexts postsContextsDataSrc,
                ServerDataAccess_PostsContextTermEntry postsContextTermEntryDataSrc,
				ClientSessionManager sessMngr
            ) : ControllerBase, ClientDataAccess_PostsContext.IAPI {
    private readonly ILogger<PostsContextController> Logger = logger;

    private readonly IServiceProvider ServiceProvider = serviceProvider;

    private readonly DbAccess DbAccess = dbAccess;

    private readonly ServerDataAccess_PostsContexts PostsContextsDataSrc = postsContextsDataSrc;

    private readonly ServerDataAccess_PostsContextTermEntry PostsContextTermEntryDataSrc = postsContextTermEntryDataSrc;

    private readonly ClientSessionManager SessionManager = sessMngr;



    [HttpPost(nameof(GetForCurrentUserByCriteria_Async))]
    public async Task<ClientDataAccess_PostsContext.IAPI.Get_Return> GetForCurrentUserByCriteria_Async(
                ClientDataAccess_PostsContext.IAPI.GetByCriteria_Params parameters ) {
        if( this.SessionManager.UserOfSession is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        TermId currUserTermId = this.SessionManager.UserAppDataOfSession?.UserDefaultTerm.Id ?? 0;

        if( !parameters.TagTermIds.Any(tid => tid == currUserTermId) ) {
            parameters.TagTermIds = parameters.TagTermIds
                .Append( currUserTermId )
                .ToArray();
        }

        IEnumerable<PostsContextObject.Raw> contexts = await this.PostsContextsDataSrc.GetByCriteria_Async(
            dbCon: dbCon,
            postsContextTermEntryDataSrc: this.PostsContextTermEntryDataSrc,
            parameters: parameters,
            alsoGetEntries: true
        );

        return new ClientDataAccess_PostsContext.IAPI.Get_Return { Contexts = contexts };
    }


    [HttpPost(nameof(CreateForCurrentUser_Async))]
    public async Task<ClientDataAccess_PostsContext.IAPI.CreateOrUpdate_Return> CreateForCurrentUser_Async(
                PostsContextObject.Prototype parameters ) {
        if( this.SessionManager.UserOfSession is null ) {
            throw new InvalidOperationException( "No user in session" );
        }
        if( !parameters.IsValid(false) ) {
            throw new ArgumentException( "Invalid PostsContextObject.Prototype in parameters." );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        return await this.PostsContextsDataSrc.Create_Async(
            dbCon,
            this.PostsContextTermEntryDataSrc,
            parameters
        );
    }

    [HttpPost(nameof(UpdateForCurrentUser_Async))]
    public async Task<ClientDataAccess_PostsContext.IAPI.CreateOrUpdate_Return> UpdateForCurrentUser_Async(
                PostsContextObject.Prototype parameters ) {
        if( this.SessionManager.UserOfSession is null ) {
            throw new InvalidOperationException( "No user in session" );
        }
        if( parameters.Id == 0 ) {
            throw new ArgumentException( "PostsContextObject.Prototype Id is not valid (must be non-null and non-zero)." );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        ClientDataAccess_PostsContext.IAPI.CreateOrUpdate_Return ret = await this.PostsContextsDataSrc.Update_Async(
            dbCon: dbCon,
            postsContextTermEntryDataSrc: this.PostsContextTermEntryDataSrc,
            parameters: parameters
        );

        return ret;
    }
}
