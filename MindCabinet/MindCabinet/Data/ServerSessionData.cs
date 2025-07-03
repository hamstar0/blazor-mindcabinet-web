using System.Security.Cryptography;

namespace MindCabinet.Data;



public class ServerSessionData {
    private readonly IHttpContextAccessor HttpContextAccessor;

    private IRequestCookieCollection? ReqCookies => this.HttpContextAccessor.HttpContext?.Request.Cookies;
    private IResponseCookies? RespCookies => this.HttpContextAccessor.HttpContext?.Response.Cookies;


    public string? SessionId { get; private set; }

    public byte[] PwSalt { get; private set; } = new byte[16];

    //public IReadOnlyList<string> RenderScripts { get; private set; }
    //
    //private IList<string> _RenderScripts = new List<string>();

    public bool IsLoaded { get; private set; } = false;



    public ServerSessionData( IHttpContextAccessor httpContextAccessor ) {
        this.HttpContextAccessor = httpContextAccessor;
        //this.RenderScripts = this._RenderScripts.AsReadOnly();
    }



    public void Load() {
        this.LoadSessionId();
        this.LoadPw();

        this.IsLoaded = true;
    }

    private void LoadSessionId() {
        if( !string.IsNullOrEmpty(this.SessionId) || this.RespCookies is null ) {
            return;
        }

        string? sessId = null;
        if( !this.ReqCookies?.TryGetValue("sessionid", out sessId) ?? true ) {
            this.SessionId = Guid.NewGuid().ToString();

            this.RespCookies.Append( "sessionid", this.SessionId );
        } else {
            this.SessionId = sessId;
        }
    }


    private void LoadPw() {
        this.PwSalt = new byte[16]; // 128 bits
        using( var rng = RandomNumberGenerator.Create() ) {
            rng.GetBytes( this.PwSalt );
        }

//        this._RenderScripts.Add(
//@"window.SessionDataFromServer = {
//    CurrentSalt: """+this.PwSalt+@""",
//    GetSessionData = function() {
//        return { CurrentSalt: window.SessionDataFromServer.CurrentSalt };
//    }
//}" );
    }
}
