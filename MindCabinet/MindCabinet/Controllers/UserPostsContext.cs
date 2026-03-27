using Microsoft.AspNetCore.Mvc;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Data;
using MindCabinet.Data.DataAccess;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserPostsContext;
using System.Data;


namespace MindCabinet.Controllers;


[ApiController]
[Route("[controller]")]
public class UserPostsContextController : ControllerBase {
    private readonly DbAccess DbAccess;

    private readonly ServerDataAccess_UserPostsContexts UserPostsContextsData;

    private readonly ServerSessionData SessionData;



    public UserPostsContextController(
                DbAccess dbAccess,
                ServerDataAccess_UserPostsContexts userPostsContextsData,
                ServerSessionData sessionData ) {
        this.DbAccess = dbAccess;
        this.UserPostsContextsData = userPostsContextsData;
        this.SessionData = sessionData;
    }


    [HttpPost(ClientDataAccess_UserPostsContext.GetForCurrentUserByCriteria_Route)]
    public async Task<ClientDataAccess_UserPostsContext.Get_Return> GetForCurrentUserByCriteria_Async(
                ClientDataAccess_UserPostsContext.GetForCurrentUserByCriteria_Params parameters ) {
        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        IEnumerable<UserPostsContextObject.Raw> contexts = await this.UserPostsContextsData.GetByCriteria_Async(
            dbCon,
            this.SessionData.UserOfSession!.Id,
            parameters,
            true
        );

        return new ClientDataAccess_UserPostsContext.Get_Return( contexts );
    }

    [HttpPost(ClientDataAccess_UserPostsContext.CreateForCurrentUser_Route)]
    public async Task<ClientDataAccess_UserPostsContext.CreateForCurrentUser_Return> CreateForCurrentUser_Async(
                UserPostsContextObject.Raw parameters ) {
        if( this.SessionData.UserOfSession is null ) {
            throw new InvalidOperationException( "No user in session" );
        }
        if( parameters.SimpleUserId != this.SessionData.UserOfSession.Id ) {
            throw new InvalidOperationException( "SimpleUserId in parameters does not match user in session." );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        return await this.UserPostsContextsData.Create_Async( dbCon, parameters );
    }

    [HttpPost(ClientDataAccess_UserPostsContext.UpdateForCurrentUser_Route)]
    public async Task<ClientDataAccess_UserPostsContext.CreateForCurrentUser_Return> UpdateForCurrentUser_Async(
                UserPostsContextObject.Raw parameters ) {
        if( this.SessionData.UserOfSession is null ) {
            throw new InvalidOperationException( "No user in session" );
        }
        if( parameters.SimpleUserId != this.SessionData.UserOfSession.Id ) {
            throw new InvalidOperationException( "SimpleUserId in parameters does not match user in session." );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        return await this.UserPostsContextsData.Update_Async( dbCon, parameters );
    }
}
