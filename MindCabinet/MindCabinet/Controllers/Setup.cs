using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.WebEncoders.Testing;
using MindCabinet.Client.Services;
using MindCabinet.Data;
using MindCabinet.Data.DataAccess;
using MindCabinet.Shared.DataObjects;
using System.Data;


namespace MindCabinet.Controllers;


[ApiController]
[Route("[controller]")]
public class SetupController : ControllerBase {
    private readonly DbAccess DbAccess;
    private readonly ServerDataAccess_Install InstallData;
    private readonly ServerDataAccess_SimpleUsers SimpleUsersData;
    private readonly ServerDataAccess_SimpleUserSessions SessionsData;
    private readonly ServerDataAccess_UserFavoriteTerms FavoriteTermsData;
    private readonly ServerDataAccess_Terms TermsData;
    private readonly ServerDataAccess_TermSets TermSetsData;
    private readonly ServerDataAccess_SimplePosts SimplePostsData;
    private readonly ServerDataAccess_UserContexts UserContextData;
    private readonly ServerDataAccess_UserAppData UserAppDataData;



    public SetupController( DbAccess dbAccess,
                ServerDataAccess_Install installData,
                ServerDataAccess_SimpleUsers simpleUsersData,
                ServerDataAccess_SimpleUserSessions sessionsData,
                ServerDataAccess_UserFavoriteTerms favoriteTermsData,
                ServerDataAccess_Terms termsData,
                ServerDataAccess_TermSets termSetsData,
                ServerDataAccess_SimplePosts simplePostsData,
                ServerDataAccess_UserContexts userContextData,
                ServerDataAccess_UserAppData userAppDataData ) {
        this.DbAccess = dbAccess;
        this.InstallData = installData;
        this.SimpleUsersData = simpleUsersData;
        this.SessionsData = sessionsData;
        this.FavoriteTermsData = favoriteTermsData;
        this.TermsData = termsData;
        this.TermSetsData = termSetsData;
        this.SimplePostsData = simplePostsData;
        this.UserContextData = userContextData;
        this.UserAppDataData = userAppDataData;
    }

    [HttpGet("Install")]
    public async Task<string> Install_Async() {
        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( false );

        if( await DbAccess.IsInstalled(dbCon) ) {
            return "Already installed.";
        }
        
        bool isInstalled = await this.InstallData.Install_Async(
            dbCon,
            this.SimpleUsersData,
            this.SessionsData,
            this.TermsData,
            this.TermSetsData,
            this.SimplePostsData,
            this.FavoriteTermsData,
            this.UserContextData,
            this.UserAppDataData
        );

        if( isInstalled ) {
            return "Success";
        } else {
            return "Failure";
        }
    }
}
