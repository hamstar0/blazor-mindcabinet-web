using Microsoft.AspNetCore.Mvc;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Data;
using MindCabinet.Data.DataAccess;
using MindCabinet.Shared.DataObjects.Term;
using System.Data;


namespace MindCabinet;


[ApiController]
[Route("[controller]")]
public class UserContextController : ControllerBase {
    private readonly DbAccess DbAccess;

    private readonly ServerDataAccess_UserContext UserContextsData;

    private readonly ServerSessionData SessionData;



    public UserContextController( DbAccess dbAccess, ServerDataAccess_UserContext userContextsData, ServerSessionData sessionData ) {
        this.DbAccess = dbAccess;
        this.UserContextsData = userContextsData;
        this.SessionData = sessionData;
    }


    [HttpPost(ClientDataAccess_UserContext.GetByUserId_Route)]
    public async Task<ClientDataAccess_UserContext.GetByUserId_Return> GetByCriteria_Async(
                ClientDataAccess_UserContext.GetByUserId_Params parameters ) {
        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async();

        return await this.UserContextsData.GetByUserId_Async( dbCon, parameters );
    }

    [HttpPost(ClientDataAccess_UserContext.CreateForCurrentUser_Route)]
    public async Task<ClientDataAccess_UserContext.CreateForCurrentUser_Return> CreateForCurrentUser_Async(
                ClientDataAccess_UserContext.CreateForCurrentUser_Params parameters ) {
        if( this.SessionData.User is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async();

        return await this.UserContextsData.Create_Async( dbCon, this.SessionData.User.Id, parameters );
    }
}
