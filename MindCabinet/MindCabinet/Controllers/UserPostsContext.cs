using Microsoft.AspNetCore.Mvc;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Data;
using MindCabinet.Data.DataAccess;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserPostsContext;
using System.Data;
using System.Text.Json;


namespace MindCabinet.Controllers;


[ApiController]
[Route("[controller]")]
public class UserPostsContextController(
                ILogger<UserPostsContextController> logger,
                DbAccess dbAccess,
                ServerDataAccess_UserPostsContexts userPostsContextsData,
                ServerSessionData sessionData ) : ControllerBase {
    private readonly ILogger<UserPostsContextController> Logger = logger;

    private readonly DbAccess DbAccess = dbAccess;

    private readonly ServerDataAccess_UserPostsContexts UserPostsContextsData = userPostsContextsData;

    private readonly ServerSessionData SessionData = sessionData;



    [HttpPost(ClientDataAccess_UserPostsContext.GetForCurrentUserByCriteria_Route)]
    public async Task<ClientDataAccess_UserPostsContext.Get_Return> GetForCurrentUserByCriteria_Async(
                ClientDataAccess_UserPostsContext.GetForCurrentUserByCriteria_Params parameters ) {
        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        IEnumerable<UserPostsContextObject.Raw> contexts = await this.UserPostsContextsData.GetByCriteria_Async(
            dbCon: dbCon,
            parameters: parameters,
            alsoGetEntries: true
        );

        return new ClientDataAccess_UserPostsContext.Get_Return( contexts );
    }

    [HttpPost(ClientDataAccess_UserPostsContext.CreateForCurrentUser_Route)]
    public async Task<ClientDataAccess_UserPostsContext.CreateOrUpdate_Return> CreateForCurrentUser_Async(
                UserPostsContextObject.Prototype parameters ) {
        if( this.SessionData.UserOfSession is null ) {
            throw new InvalidOperationException( "No user in session" );
        }
        if( !parameters.IsValid(false) ) {
            throw new ArgumentException( "Invalid UserPostsContextObject.Prototype in parameters." );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        return await this.UserPostsContextsData.Create_Async( dbCon, parameters );
    }

    [HttpPost(ClientDataAccess_UserPostsContext.UpdateForCurrentUser_Route)]
    public async Task<ClientDataAccess_UserPostsContext.CreateOrUpdate_Return> UpdateForCurrentUser_Async(
                UserPostsContextObject.Prototype parameters ) {
        if( this.SessionData.UserOfSession is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        return await this.UserPostsContextsData.Update_Async( dbCon, parameters );
    }
}
