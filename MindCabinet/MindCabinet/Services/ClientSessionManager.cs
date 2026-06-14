using MindCabinet.Data;
using MindCabinet.Data.DataAccess;
using MindCabinet.Shared.DataObjects;
using System.Data;

namespace MindCabinet.Services;



public partial class ClientSessionManager(
            ILogger<ClientSessionManager> logger,
            IHttpContextAccessor httpContext ) {
    internal static async Task LoadForHubRequest_Async( IServiceProvider services ) {
        var sessionData = services.GetRequiredService<ClientSessionManager>();
        var dbAccess = services.GetRequiredService<DbAccess>();
        var termsDataSrc = services.GetRequiredService<ServerDataAccess_Terms>();
        var usersDataSrc = services.GetRequiredService<ServerDataAccess_SimpleUsers>();
        var userAppDataSrc = services.GetRequiredService<ServerDataAccess_UserAppData>();
        var postsContextsDataSrc = services.GetRequiredService<ServerDataAccess_PostsContexts>();
        var postsContextTermEntryDataSrc = services.GetRequiredService<ServerDataAccess_PostsContextTermEntry>();
        using var dbCon = await dbAccess.GetDbConnection_Async( true );

        await sessionData.LoadForHubRequest_Async(
            dbCon: dbCon,
            termsDataSrc: termsDataSrc,
            usersDataSrc: usersDataSrc,
            userAppDataSrc: userAppDataSrc,
            postsContextsDataSrc: postsContextsDataSrc,
            postsContextTermEntryDataSrc: postsContextTermEntryDataSrc
        );
    }



    private readonly ILogger<ClientSessionManager> Logger = logger;

    private readonly IHttpContextAccessor HttpContext = httpContext;

    
    private IRequestCookieCollection? ReqCookies => this.HttpContext.HttpContext?.Request.Cookies;
    private IResponseCookies? RespCookies => this.HttpContext.HttpContext?.Response.Cookies;


    public string? CurrentSessionId { get; private set; }

    public string? CurrentIpAddress { get; private set; }

    
    public SimpleUserObject? UserOfSession { get; private set; }

    public UserAppDataObject? UserAppDataOfSession { get; private set; }



    public bool IsLoaded => this.CurrentSessionId is not null;



    public async Task<bool> LoadForHttpRequest_Async(
                IDbConnection dbCon,
                ServerDataAccess_Terms termsDataSrc,
                ServerDataAccess_SimpleUsers usersDataSrc,
                ServerDataAccess_UserAppData userAppDataSrc,
                ServerDataAccess_PostsContexts postsContextsDataSrc,
                ServerDataAccess_PostsContextTermEntry postsContextTermEntryDataSrc,
                bool isInstalling ) {
        if( !string.IsNullOrEmpty(this.CurrentSessionId) ) {
            return false;
        }
        if( this.RespCookies is null ) {
            return false;
        }

        this.LoadIp();

        string? sessId = null;
        this.ReqCookies?.TryGetValue( "sessionid", out sessId );

        bool isLoggedIn = false;

        if( sessId is null ) {
            this.LoadNewSessionAndNoUser();
        } else {
            isLoggedIn = await this.LoadExistingSessionAndItsUser_Async(
                dbCon: dbCon,
                termsDataSrc: termsDataSrc,
                usersDataSrc: usersDataSrc,
                userAppDataSrc: userAppDataSrc,
                postsContextsDataSrc: postsContextsDataSrc,
                postsContextTermEntryDataSrc: postsContextTermEntryDataSrc,
                sessId: sessId,
                isInstalling: isInstalling
            );
        }
// this.Logger.LogInformation( $"SESS: {sessId}, IP: {this.IpAddress}, User: {this.UserOfSession?.Name} ({isLoggedIn})" );

        return isLoggedIn;
    }

    public async Task<bool> LoadForHubRequest_Async(
                IDbConnection dbCon,
                ServerDataAccess_Terms termsDataSrc,
                ServerDataAccess_SimpleUsers usersDataSrc,
                ServerDataAccess_UserAppData userAppDataSrc,
                ServerDataAccess_PostsContexts postsContextsDataSrc,
                ServerDataAccess_PostsContextTermEntry postsContextTermEntryDataSrc ) {
        if( this.IsLoaded ) {
            return false;
        }
        
        this.LoadIp();

        string? sessId = null;
        this.ReqCookies?.TryGetValue( "sessionid", out sessId );
        if( sessId is null ) {
            throw new InvalidOperationException( "No session ID in cookies." );
        }

        bool isLoggedIn = await this.LoadExistingSessionAndItsUser_Async(
            dbCon: dbCon,
            termsDataSrc: termsDataSrc,
            usersDataSrc: usersDataSrc,
            userAppDataSrc: userAppDataSrc,
            postsContextsDataSrc: postsContextsDataSrc,
            postsContextTermEntryDataSrc: postsContextTermEntryDataSrc,
            sessId: sessId,
            isInstalling: false
        );

        return isLoggedIn;
    }


    private void LoadIp() {
        this.CurrentIpAddress = this.HttpContext.HttpContext?.Connection.RemoteIpAddress?.ToString();

        if( string.IsNullOrEmpty(this.CurrentIpAddress) ) {
            throw new Exception( "Who are you?" );
        }
    }

    private void LoadNewSessionAndNoUser() {
        this.CurrentSessionId = Guid.NewGuid().ToString();
        this.RespCookies?.Append( "sessionid", this.CurrentSessionId );
    }

    private async Task<bool> LoadExistingSessionAndItsUser_Async(
                IDbConnection dbCon,
                ServerDataAccess_Terms termsDataSrc,
                ServerDataAccess_SimpleUsers usersDataSrc,
                ServerDataAccess_UserAppData userAppDataSrc,
                ServerDataAccess_PostsContexts postsContextsDataSrc,
                ServerDataAccess_PostsContextTermEntry postsContextTermEntryDataSrc,
                string sessId,
                bool isInstalling ) {
        this.CurrentSessionId = sessId;

        if( isInstalling ) {
            return false;
        }

        bool isLoggedIn = await this.LoadUserOfSession_Async( dbCon, usersDataSrc, sessId!, this.CurrentIpAddress! );

        if( isLoggedIn ) {
            UserAppDataObject.Raw? userAppDataRaw = await userAppDataSrc.GetById_Async(
                dbCon: dbCon,
                id: this.UserOfSession!.Id
            );

            this.UserAppDataOfSession = userAppDataRaw is not null
                ? await ServerDataAccess_UserAppData.ToDataObject_Async(
                    dbCon: dbCon,
                    termsDataSrc: termsDataSrc,
                    postsContextsDataSrc: postsContextsDataSrc,
                    postsContextTermEntryDataSrc: postsContextTermEntryDataSrc,
                    dbEntry: userAppDataRaw
                )
                : null;
        }

        return isLoggedIn;
    }

    
    public async Task LogoutSessionAndItsUser_Async( IDbConnection dbCon, ServerDataAccess_SimpleUserSessions sessionsData ) {
        await this.LogoutUser_Async( dbCon, sessionsData );
        
        //this.RespCookies?.Delete( "sessionid" );

        //this.SessionId = null;
    }
}
