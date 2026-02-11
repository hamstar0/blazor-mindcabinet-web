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

    public bool IsLoaded { get; private set; } = false;



    public async Task<bool> Load_Async( IDbConnection dbCon, ServerDataAccess_SimpleUsers userData, bool isInstalling ) {
        if( !string.IsNullOrEmpty(this.SessionId) || this.RespCookies is null ) {
            return false;
        }

        string? sessId = null;
        this.ReqCookies?.TryGetValue( "sessionid", out sessId );

        bool isLoggedIn = false;

        if( sessId is null ) {
            this.LoadNewSession( isInstalling );
        } else {
            isLoggedIn = await this.LoadExistingSession( dbCon, userData, sessId, isInstalling );
        }

        this.IsLoaded = true;

        return isLoggedIn;
    }


    private void LoadNewSession( bool isInstalling ) {
        this.SessionId = Guid.NewGuid().ToString();
        this.IpAddress = this.HttpContext.HttpContext?.Connection.RemoteIpAddress?.ToString();

        this.RespCookies?.Append( "sessionid", this.SessionId );
    }

    private async Task<bool> LoadExistingSession(
                IDbConnection dbCon,
                ServerDataAccess_SimpleUsers userData,
                string sessId,
                bool isInstalling ) {
        this.SessionId = sessId;
        this.IpAddress = this.HttpContext.HttpContext?.Connection.RemoteIpAddress?.ToString();

        return isInstalling
            ? false
            : await this.LoadUserOfSession_Async( dbCon, userData, sessId! );
    }
}
