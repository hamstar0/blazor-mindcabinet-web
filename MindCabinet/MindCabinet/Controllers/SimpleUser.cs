using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Data;
using MindCabinet.Data.DataAccess;
using MindCabinet.Shared.DataObjects;
using System.Data;
using System.Security.Cryptography;
using System.Text;


namespace MindCabinet;


[ApiController]
[Route("[controller]")]
public partial class SimpleUserController : ControllerBase {
    private readonly DbAccess DbAccess;

    private readonly ServerDataAccess_SimpleUsers SimpleUsersData;

    private readonly ServerDataAccess_SimpleUsers_Sessions SessionsData;

    private readonly ServerDataAccess_UserFavoriteTerms FavoriteTermsData;
    
    private readonly ServerSessionData ServerSessionData;



    public SimpleUserController(
                DbAccess dbAccess,
                ServerDataAccess_SimpleUsers simpleUsersData,
                ServerDataAccess_SimpleUsers_Sessions sessionsData,
                ServerDataAccess_UserFavoriteTerms favoriteTermsData,
                ServerSessionData sessData ) {
        this.DbAccess = dbAccess;
        this.SimpleUsersData = simpleUsersData;
        this.SessionsData = sessionsData;
        this.FavoriteTermsData = favoriteTermsData;
        this.ServerSessionData = sessData;
    }

    [HttpPost(ClientDataAccess_SimpleUsers.Create_Route)]
    public async Task<ClientDataAccess_SimpleUsers.Login_Return> Create_Async(
                ClientDataAccess_SimpleUsers.Create_Params parameters ) {
        if( !this.ServerSessionData.IsLoaded ) {
            throw new NullReferenceException( "Session not loaded." );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async();

        ServerDataAccess_SimpleUsers.SimpleUserQueryResult result = await this.SimpleUsersData.CreateSimpleUser_Async(
            dbCon: dbCon,
            parameters: parameters,
            pwSalt: this.ServerSessionData.PwSalt!
        );

        if( result.User is not null ) {
            await this.SessionsData.CreateSimpleUserSession_Async(
                dbCon: dbCon,
                user: result.User,
                session: this.ServerSessionData
            );
        }

        return new ClientDataAccess_SimpleUsers.Login_Return(
            result.User?.GetClientOnlyData(),
            result.User is not null ? "User created." : "Could not create user."
        );
    }
}
