using MindCabinet.Shared.DataObjects;
using System.Data;
using System.Security.Cryptography;

namespace MindCabinet.Data;



public partial class ServerSessionData {
    private readonly IHttpContextAccessor Http;
    
    private readonly ServerDbAccess Db;

    private IRequestCookieCollection? ReqCookies => this.Http.HttpContext?.Request.Cookies;
    private IResponseCookies? RespCookies => this.Http.HttpContext?.Response.Cookies;


    public string? SessionId { get; private set; }

    public string? IpAddress { get; private set; }

    //public IReadOnlyList<string> RenderScripts { get; private set; }
    //
    //private IList<string> _RenderScripts = new List<string>();

    public bool IsLoaded { get; private set; } = false;



    public ServerSessionData( IHttpContextAccessor http, ServerDbAccess db ) {
        this.Http = http;
        this.Db = db;
        //this.RenderScripts = this._RenderScripts.AsReadOnly();
    }



    public async Task<bool> Load_Async( IDbConnection dbCon ) {
        if( !string.IsNullOrEmpty(this.SessionId) || this.RespCookies is null ) {
            return false;
        }

        string? sessId = null;
        this.ReqCookies?.TryGetValue( "sessionid", out sessId );

        bool isLoggedIn = sessId is not null;

        if( isLoggedIn ) {
            isLoggedIn = await this.LoadCurrentSessionUser_Async( dbCon, sessId! );
        }

        if( !isLoggedIn ) {
            sessId = Guid.NewGuid().ToString();

            this.LoadNewSession( sessId );
        }

        this.IsLoaded = true;

        return isLoggedIn;
    }


    private void LoadNewSession( string sessId ) {
        this.SessionId = sessId;
        this.IpAddress = this.Http.HttpContext?.Connection.RemoteIpAddress?.ToString();

        this.PwSalt = new byte[16]; // 128 bits

        using( var rng = RandomNumberGenerator.Create() ) {
            rng.GetBytes( this.PwSalt );
        }

        this.RespCookies?.Append( "sessionid", this.SessionId );
//        this._RenderScripts.Add(
//@"window.SessionDataFromServer = {
//    CurrentSalt: """+this.PwSalt+@""",
//    GetSessionData = function() {
//        return { CurrentSalt: window.SessionDataFromServer.CurrentSalt };
//    }
//}" );
    }
}
