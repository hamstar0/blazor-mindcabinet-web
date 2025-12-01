using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using MindCabinet.Client.Services;
using MindCabinet.Data;
using MindCabinet.Data.DataAccess;
using MindCabinet.Shared.DataObjects;
using System.Data;
using System.Security.Cryptography;
using System.Text;


namespace MindCabinet;


[ApiController]
[Route("[controller]")]
public class SessionController : ControllerBase {
    private DbAccess DbAccess;
    private ServerDataAccess_Terms TermsData;
    private ServerSessionData SessData;



    public SessionController( DbAccess dbAccess, ServerDataAccess_Terms termsData, ServerSessionData sessData ) {
        this.DbAccess = dbAccess;
        this.TermsData = termsData;
        this.SessData = sessData;
    }

    // [HttpPost(nameof(ClientDbAccess.Route_SimpleUser_GetSessionData.route))]
    [HttpGet(ClientSessionData.Get_Route)]
    public async Task<ClientSessionData.RawServerData> Get_Async() {
        if( !this.SessData.IsLoaded ) {
            throw new NullReferenceException( "Session not loaded." );
        }

        return new ClientSessionData.RawServerData(
            sessionId: this.SessData.SessionId!,
            userData: this.SessData.User?.GetClientOnlyData()
        );
    }
}
