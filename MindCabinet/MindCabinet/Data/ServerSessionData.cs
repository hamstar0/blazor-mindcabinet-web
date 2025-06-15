namespace MindCabinet.Data;



public class ServerSessionData {
    private readonly IHttpContextAccessor HttpContextAccessor;

    private IRequestCookieCollection? ReqCookies => this.HttpContextAccessor.HttpContext?.Request.Cookies;
    private IResponseCookies? RespCookies => this.HttpContextAccessor.HttpContext?.Response.Cookies;

    public string? SessionId { get; private set; }

    public string? PwSalt { get; private set; }

    public IReadOnlyList<string> RenderScripts { get; private set; }

    private IList<string> _RenderScripts = new List<string>();



    public ServerSessionData( IHttpContextAccessor httpContextAccessor ) {
        this.HttpContextAccessor = httpContextAccessor;
        this.RenderScripts = this._RenderScripts.AsReadOnly();
    }



    public void Load() {
        this.LoadSessionId();
        this.LoadPw();
    }

    private void LoadSessionId() {
        if( !string.IsNullOrEmpty(this.SessionId) || this.RespCookies is null ) {
            return;
        }

        string? sessId = null;
        if( !this.ReqCookies?.TryGetValue("sessionid", out sessId) ?? true ) {
            this.SessionId = System.Guid.NewGuid().ToString();

            this.RespCookies.Append( "sessionid", this.SessionId );
        } else {
            this.SessionId = sessId;
        }
    }


    private void LoadPw() {
        if( !string.IsNullOrEmpty(this.PwSalt) ) {
            return;
        }

        this.PwSalt = System.Guid.NewGuid().ToString();

        this._RenderScripts.Add( "window.ServerData = { PwSalt: \""+this.PwSalt+"\" };" );
    }
}
