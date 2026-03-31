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


namespace MindCabinet.Controllers;


[ApiController]
[Route("[controller]")]
public partial class SimpleUserController : ControllerBase {
    private readonly ILogger<SimpleUserController> Logger;

    private readonly DbAccess DbAccess;

    private readonly ServerDataAccess_SimpleUsers SimpleUsersData;

    private readonly ServerDataAccess_SimpleUserSessions SessionsData;

    private readonly ServerDataAccess_UserTermFavorites FavoriteTermsData;
    
    private readonly ServerSessionData ServerSessionData;



    public SimpleUserController(
                ILogger<SimpleUserController> logger,
                DbAccess dbAccess,
                ServerDataAccess_SimpleUsers simpleUsersData,
                ServerDataAccess_SimpleUserSessions sessionsData,
                ServerDataAccess_UserTermFavorites favoriteTermsData,
                ServerSessionData sessData ) {
        this.Logger = logger;
        this.DbAccess = dbAccess;
        this.SimpleUsersData = simpleUsersData;
        this.SessionsData = sessionsData;
        this.FavoriteTermsData = favoriteTermsData;
        this.ServerSessionData = sessData;
    }

    [HttpPost(ClientDataAccess_SimpleUsers.Create_Route)]
    public async Task<ClientDataAccess_SimpleUsers.Create_Return> Create_Async(
                ClientDataAccess_SimpleUsers.Create_Params parameters ) {
        if( parameters.IsValidated ) {
            return new ClientDataAccess_SimpleUsers.Create_Return { User = null, Status = "Not permitted." };
        }
        if( !this.ServerSessionData.IsLoaded ) {
            throw new NullReferenceException( "Session not loaded." );
        }

        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        ServerDataAccess_SimpleUsers.SimpleUserQueryResult result = await this.SimpleUsersData.CreateSimpleUser_Async(
            dbCon: dbCon,
            parameters: parameters,
            detectCollision: true
        );

        // if( result.User is not null ) {      <- Do not log in automatically!
        //     await this.SessionsData.CreateSimpleUserSession_Async(
        //         dbCon: dbCon,
        //         user: result.User,
        //         session: this.ServerSessionData
        //     );
        // }
        this.Logger.LogInformation( $"User already exists? {result.AlreadyExists}" );

        return new ClientDataAccess_SimpleUsers.Create_Return {
            User = result.User is not null
                ? new SimpleUserObject.ClientObject( result.User.Id, result.User.Name, result.User.Created, result.User.Email )
                : null,
            Status = result.User is not null
                ? "User created. Validate email address and log in to complete registration."
                : "Could not create user."
        };
    }
}
