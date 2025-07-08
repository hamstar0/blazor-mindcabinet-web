using MindCabinet.Shared.DataEntries;
using System.Security.Cryptography;

namespace MindCabinet.Data;



public class ServerSessionData {
    private readonly IHttpContextAccessor HttpContextAccessor;
    
    private readonly ServerDbAccess Db;

    private IRequestCookieCollection? ReqCookies => this.HttpContextAccessor.HttpContext?.Request.Cookies;
    private IResponseCookies? RespCookies => this.HttpContextAccessor.HttpContext?.Response.Cookies;


    public string? SessionId { get; private set; }

    public SimpleUserEntry? User { get; private set; }

    public byte[] PwSalt { get; private set; } = new byte[16];

    //public IReadOnlyList<string> RenderScripts { get; private set; }
    //
    //private IList<string> _RenderScripts = new List<string>();

    public bool IsLoaded { get; private set; } = false;



    public ServerSessionData( IHttpContextAccessor httpContextAccessor, ServerDbAccess db ) {
        this.HttpContextAccessor = httpContextAccessor;
        this.Db = db;
        //this.RenderScripts = this._RenderScripts.AsReadOnly();
    }



    public async Task<bool> Load_Async() {
        if( !string.IsNullOrEmpty(this.SessionId) || this.RespCookies is null ) {
            return false;
        }

        string? sessId = null;
        this.ReqCookies?.TryGetValue( "sessionid", out sessId );

        if( sessId is null ) {
            this.LoadNewSession( Guid.NewGuid().ToString() );
        } else {
            await this.LoadCurrentSessionUser_Async( sessId );
        }

        this.IsLoaded = true;

        return sessId is not null;
    }


    private void LoadNewSession( string sessId ) {
        this.SessionId = Guid.NewGuid().ToString();

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

    private async Task LoadCurrentSessionUser_Async( string sessId ) {
        this.SessionId = sessId;

        this.User = await this.Db.GetSimpleUserBySession_Async( sessId );
    }
}
