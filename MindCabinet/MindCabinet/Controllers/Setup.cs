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
    private readonly ServerDataAccess_Install InstallDataSrc;
    private readonly ServerDataAccess_SimpleUsers SimpleUsersDataSrc;
    private readonly ServerDataAccess_SimpleUserSessions SessionsDataSrc;
    private readonly ServerDataAccess_UserTermFavorites FavoriteTermsDataSrc;
    private readonly ServerDataAccess_UserTermsHistory HistoryTermsDataSrc;
    private readonly ServerDataAccess_Terms TermsDataSrc;
    private readonly ServerDataAccess_SimplePostTags TermSetsDataSrc;
    private readonly ServerDataAccess_SimplePosts SimplePostsDataSrc;
    private readonly ServerDataAccess_PostsContexts PostsContextDataSrc;
    private readonly ServerDataAccess_PostsContextTermEntry PostsContextTermEntryDataSrc;
    private readonly ServerDataAccess_UserAppData UserAppDataDataSrc;
    private readonly ServerDataAccess_ServerData ServerDataSrc;



    public SetupController( DbAccess dbAccess,
                ServerDataAccess_Install installDataSrc,
                ServerDataAccess_SimpleUsers simpleUsersDataSrc,
                ServerDataAccess_SimpleUserSessions sessionsDataSrc,
                ServerDataAccess_UserTermFavorites favoriteTermsDataSrc,
                ServerDataAccess_UserTermsHistory historyTermsDataSrc,
                ServerDataAccess_Terms termsDataSrc,
                ServerDataAccess_SimplePostTags termSetsDataSrc,
                ServerDataAccess_SimplePosts simplePostsDataSrc,
                ServerDataAccess_PostsContexts postsContextDataSrc,
                ServerDataAccess_PostsContextTermEntry postsContextTermEntryDataSrc,
                ServerDataAccess_UserAppData userAppDataDataSrc,
                ServerDataAccess_ServerData serverDataSrc ) {
        this.DbAccess = dbAccess;
        this.InstallDataSrc = installDataSrc;
        this.SimpleUsersDataSrc = simpleUsersDataSrc;
        this.SessionsDataSrc = sessionsDataSrc;
        this.FavoriteTermsDataSrc = favoriteTermsDataSrc;
        this.HistoryTermsDataSrc = historyTermsDataSrc;
        this.TermsDataSrc = termsDataSrc;
        this.TermSetsDataSrc = termSetsDataSrc;
        this.SimplePostsDataSrc = simplePostsDataSrc;
        this.PostsContextDataSrc = postsContextDataSrc;
        this.PostsContextTermEntryDataSrc = postsContextTermEntryDataSrc;
        this.UserAppDataDataSrc = userAppDataDataSrc;
        this.ServerDataSrc = serverDataSrc;
    }
    

    [HttpGet("Install")]
    public async Task<string> Install_Async() {
        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( false );

        if( await DbAccess.IsInstalled(dbCon) ) {
            return "Already installed.";
        }
        
        bool isInstalled = await this.InstallDataSrc.Install_Async(
            dbCon,
            this.SimpleUsersDataSrc,
            this.SessionsDataSrc,
            this.TermsDataSrc,
            this.TermSetsDataSrc,
            this.SimplePostsDataSrc,
            this.FavoriteTermsDataSrc,
            this.HistoryTermsDataSrc,
            this.PostsContextDataSrc,
            this.PostsContextTermEntryDataSrc,
            this.UserAppDataDataSrc,
            this.ServerDataSrc
        );

        if( isInstalled ) {
            return "Success";
        } else {
            return "Failure";
        }
    }
}
