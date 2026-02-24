using MindCabinet.Data.DataAccess;
using MindCabinet.Services;
using MindCabinet.Shared.DataObjects;
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


    public string? SessionId { get; private set; }

    public string? IpAddress { get; private set; }

    //public IReadOnlyList<string> RenderScripts { get; private set; }
    //
    //private IList<string> _RenderScripts = new List<string>();
    //this.RenderScripts = this._RenderScripts.AsReadOnly();

    
    public SimpleUserObject? UserOfSession { get; private set; }

    public UserAppDataObject? UserAppData { get; private set; }



    public bool IsLoaded { get; private set; } = false;



    public async Task<bool> Load_Async( IDbConnection dbCon, ServerDataAccess_SimpleUsers userData, bool isInstalling ) {
        if( !string.IsNullOrEmpty(this.SessionId) || this.RespCookies is null ) {
            return false;
        }

        this.LoadIp();

        string? sessId = null;
        this.ReqCookies?.TryGetValue( "sessionid", out sessId );

        bool isLoggedIn = false;

        if( sessId is null ) {
            this.LoadNewSessionAndNoUser();
        } else {
            isLoggedIn = await this.LoadExistingSessionAndItsUser_Async( dbCon, userData, sessId, isInstalling );
        }
// this.Logger.LogInformation( $"SESS: {sessId}, IP: {this.IpAddress}, User: {this.UserOfSession?.Name} ({isLoggedIn})" );

        this.IsLoaded = true;

        return isLoggedIn;
    }


    private void LoadIp() {
        this.IpAddress = this.HttpContext.HttpContext?.Connection.RemoteIpAddress?.ToString();

        if( string.IsNullOrEmpty(this.IpAddress) ) {
            throw new Exception( "Who are you?" );
        }
    }

    private void LoadNewSessionAndNoUser() {
        this.SessionId = Guid.NewGuid().ToString();
        this.RespCookies?.Append( "sessionid", this.SessionId );
    }

    private async Task<bool> LoadExistingSessionAndItsUser_Async(
                IDbConnection dbCon,
                ServerDataAccess_SimpleUsers userData,
                ServerDataAccess_UserAppData userAppData,
                string sessId,
                bool isInstalling ) {
        this.SessionId = sessId;

        if( isInstalling ) {
            return false;
        }

        bool isLoggedIn = await this.LoadUserOfSession_Async( dbCon, userData, sessId!, this.IpAddress! );

        if( isLoggedIn ) {
            this.UserAppData = await userAppData.GetById_Async( dbCon, this.UserOfSession!.Id );
        }

        return isLoggedIn;
    }

    
    public async Task LogoutSessionAndItsUser_Async( IDbConnection dbCon, ServerDataAccess_SimpleUserSessions sessionsData ) {
        await this.LogoutUser_Async( dbCon, sessionsData );
        
        //this.RespCookies?.Delete( "sessionid" );

        //this.SessionId = null;
    }
}
