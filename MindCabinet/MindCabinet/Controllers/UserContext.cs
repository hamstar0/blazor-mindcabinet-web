using Microsoft.AspNetCore.Mvc;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Data;
using MindCabinet.Data.DataAccess;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserContext;
using System.Data;


namespace MindCabinet.Controllers;


[ApiController]
[Route("[controller]")]
public class UserContextController : ControllerBase {
    private readonly DbAccess DbAccess;

    private readonly ServerDataAccess_UserContext UserContextsData;

    private readonly ServerSessionData SessionData;



    public UserContextController(
                DbAccess dbAccess,
                ServerDataAccess_UserContext userContextsData,
                ServerSessionData sessionData ) {
        this.DbAccess = dbAccess;
        this.UserContextsData = userContextsData;
        this.SessionData = sessionData;
    }


    [HttpPost(ClientDataAccess_UserContext.GetForCurrentUserByCriteria_Route)]
    public async Task<ClientDataAccess_UserContext.Get_Return> GetForCurrentUserByCriteria_Async(
                ClientDataAccess_UserContext.GetForCurrentUserByCriteria_Params parameters ) {
        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        return await this.UserContextsData.GetByCriteria_Async( dbCon, this.SessionData.User!.Id, parameters );
    }

    [HttpPost(ClientDataAccess_UserContext.CreateForCurrentUser_Route)]
    public async Task<ClientDataAccess_UserContext.CreateForCurrentUser_Return> CreateForCurrentUser_Async(
                UserContextObject.DatabaseEntry parameters ) {
        if( this.SessionData.User is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        return await this.UserContextsData.Create_Async( dbCon, this.SessionData.User.Id, parameters );
    }

    [HttpPost(ClientDataAccess_UserContext.UpdateForCurrentUser_Route)]
    public async Task<ClientDataAccess_UserContext.CreateForCurrentUser_Return> UpdateForCurrentUser_Async(
                UserContextObject.DatabaseEntry parameters ) {
        if( this.SessionData.User is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        return await this.UserContextsData.Update_Async( dbCon, this.SessionData.User.Id, parameters );
    }
}
