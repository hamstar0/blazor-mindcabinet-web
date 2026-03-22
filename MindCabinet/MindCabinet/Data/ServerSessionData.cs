using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Data.DataAccess;
using MindCabinet.DataObjects;
using MindCabinet.Services;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.UserContext;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace MindCabinet.Data;



public partial class ServerSessionData(
            ILogger<ServerSessionData> logger,
            IHttpContextAccessor httpContext ) {
    private readonly ILogger<ServerSessionData> Logger = logger;

    private readonly IHttpContextAccessor HttpContext = httpContext;

    
    private IRequestCookieCollection? ReqCookies => this.HttpContext.HttpContext?.Request.Cookies;
    private IResponseCookies? RespCookies => this.HttpContext.HttpContext?.Response.Cookies;


    public string? CurrentSessionId { get; private set; }

    public string? CurrentIpAddress { get; private set; }

    
    public SimpleUserObject? UserOfSession { get; private set; }

    public UserAppDataObject? UserAppDataOfSession { get; private set; }



    public bool IsLoaded { get; private set; } = false;



    public async Task<bool> Load_Async(
                IDbConnection dbCon,
                ServerDataAccess_Terms termsData,
                ServerDataAccess_SimpleUsers userData,
                ServerDataAccess_UserAppData userAppData,
                ServerDataAccess_UserContexts userContextsData,
                bool isInstalling ) {
        if( !string.IsNullOrEmpty(this.CurrentSessionId) || this.RespCookies is null ) {
            return false;
        }

        this.LoadIp();

        string? sessId = null;
        this.ReqCookies?.TryGetValue( "sessionid", out sessId );

        bool isLoggedIn = false;

        if( sessId is null ) {
            this.LoadNewSessionAndNoUser();
        } else {
            isLoggedIn = await this.LoadExistingSessionAndItsUser_Async( dbCon, termsData, userData, userAppData, userContextsData, sessId, isInstalling );
        }
// this.Logger.LogInformation( $"SESS: {sessId}, IP: {this.IpAddress}, User: {this.UserOfSession?.Name} ({isLoggedIn})" );

        this.IsLoaded = true;

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
                ServerDataAccess_Terms termsData,
                ServerDataAccess_SimpleUsers userData,
                ServerDataAccess_UserAppData userAppData,
                ServerDataAccess_UserContexts userContextsData,
                string sessId,
                bool isInstalling ) {
        this.CurrentSessionId = sessId;

        if( isInstalling ) {
            return false;
        }

        bool isLoggedIn = await this.LoadUserOfSession_Async( dbCon, userData, sessId!, this.CurrentIpAddress! );

        if( isLoggedIn ) {
            UserAppDataObject.Raw? userAppDataRaw = await userAppData.GetById_Async( dbCon, this.UserOfSession!.Id );

            this.UserAppDataOfSession = userAppDataRaw is not null
                ? await ServerDataAccess_UserAppData.ToObject_Async( dbCon, termsData, userContextsData, userAppDataRaw )
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
