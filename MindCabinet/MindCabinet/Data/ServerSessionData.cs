using MindCabinet.Data.DataAccess;
using MindCabinet.Shared.DataObjects;
using System.Data;
using System.Security.Cryptography;

namespace MindCabinet.Data;



public partial class ServerSessionData {
    private readonly ILogger<ServerSessionData> Logger;

    private readonly IHttpContextAccessor HttpContext;

    private readonly ServerDataAccess_SimpleUsers UserData;

    
    private IRequestCookieCollection? ReqCookies => this.HttpContext.HttpContext?.Request.Cookies;
    private IResponseCookies? RespCookies => this.HttpContext.HttpContext?.Response.Cookies;


    public string? SessionId { get; private set; }

    public string? IpAddress { get; private set; }

    //public IReadOnlyList<string> RenderScripts { get; private set; }
    //
    //private IList<string> _RenderScripts = new List<string>();
    //this.RenderScripts = this._RenderScripts.AsReadOnly();

    public bool IsLoaded { get; private set; } = false;



    public ServerSessionData( ILogger<ServerSessionData> logger, IHttpContextAccessor httpContext, ServerDataAccess_SimpleUsers userData ) {
        this.Logger = logger;
        this.HttpContext = httpContext;
        this.UserData = userData;
    }

    public async Task<bool> Load_Async( IDbConnection dbCon ) {
        if( !string.IsNullOrEmpty(this.SessionId) || this.RespCookies is null ) {
            return false;
        }

        string? sessId = null;
        this.ReqCookies?.TryGetValue( "sessionid", out sessId );


        bool isLoggedIn = false;

        if( sessId is null ) {
            this.LoadNewSession();
        } else {
            isLoggedIn = await this.LoadExistingSession( dbCon, sessId! );
        }

        this.IsLoaded = true;

        return isLoggedIn;
    }


    private void LoadNewSession() {
        this.SessionId = Guid.NewGuid().ToString();
        this.IpAddress = this.HttpContext.HttpContext?.Connection.RemoteIpAddress?.ToString();

        this.PwSalt = new byte[16]; // 128 bits

        using( var rng = RandomNumberGenerator.Create() ) {
            rng.GetBytes( this.PwSalt );
        }

        this.RespCookies?.Append( "sessionid", this.SessionId );
    }

    private async Task<bool> LoadExistingSession( IDbConnection dbCon, string sessId ) {
        this.SessionId = sessId;
            
        return await this.LoadUserOfSession_Async( dbCon, this.UserData, sessId! );
    }
}
