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


namespace MindCabinet.Controllers;


[ApiController]
[Route("[controller]")]
public class PostsContextController(
                ILogger<PostsContextController> logger,
                DbAccess dbAccess,
                ServerDataAccess_PostsContexts postsContextsData,
                ServerDataAccess_PostsContextTermEntry postsContextTermEntryData,
                ServerSessionManager sessMngr ) : ControllerBase {
    private readonly ILogger<PostsContextController> Logger = logger;

    private readonly DbAccess DbAccess = dbAccess;

    private readonly ServerDataAccess_PostsContexts PostsContextsData = postsContextsData;

    private readonly ServerDataAccess_PostsContextTermEntry PostsContextTermEntryData = postsContextTermEntryData;

    private readonly ServerSessionManager SessionManager = sessMngr;



    [HttpPost(ClientDataAccess_PostsContext.GetForCurrentUserByCriteria_Route)]
    public async Task<ClientDataAccess_PostsContext.Get_Return> GetForCurrentUserByCriteria_Async(
                ClientDataAccess_PostsContext.GetForCurrentUserByCriteria_Params parameters ) {
        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        IEnumerable<PostsContextObject.Raw> contexts = await this.PostsContextsData.GetByCriteria_Async(
            dbCon: dbCon,
            postsContextTermEntryData: this.PostsContextTermEntryData,
            parameters: parameters,
            alsoGetEntries: true
        );

        return new ClientDataAccess_PostsContext.Get_Return { Contexts = contexts };
    }

    [HttpPost(ClientDataAccess_PostsContext.CreateForCurrentUser_Route)]
    public async Task<ClientDataAccess_PostsContext.CreateOrUpdate_Return> CreateForCurrentUser_Async(
                PostsContextObject.Prototype parameters ) {
        if( this.SessionManager.UserOfSession is null ) {
            throw new InvalidOperationException( "No user in session" );
        }
        if( !parameters.IsValid(false) ) {
            throw new ArgumentException( "Invalid PostsContextObject.Prototype in parameters." );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        return await this.PostsContextsData.Create_Async(
            dbCon,
            this.PostsContextTermEntryData,
            parameters
        );
    }

    [HttpPost(ClientDataAccess_PostsContext.UpdateForCurrentUser_Route)]
    public async Task<ClientDataAccess_PostsContext.CreateOrUpdate_Return> UpdateForCurrentUser_Async(
                PostsContextObject.Prototype parameters ) {
        if( this.SessionManager.UserOfSession is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        return await this.PostsContextsData.Update_Async(
            dbCon,
            this.PostsContextTermEntryData,
            parameters
        );
    }
}
