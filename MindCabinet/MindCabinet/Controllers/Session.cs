using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using MindCabinet.Client.Services;
using MindCabinet.Data;
using MindCabinet.Shared.DataObjects;
using System.Data;
using System.Security.Cryptography;
using System.Text;


namespace MindCabinet;


[ApiController]
[Route("[controller]")]
public class SessionController : ControllerBase {
    private ServerDbAccess DbAccess;
    private ServerSessionData SessData;



    public SessionController( ServerDbAccess dbAccess, ServerSessionData sessData ) {
        this.DbAccess = dbAccess;
        this.SessData = sessData;
    }

    // [HttpPost(nameof(ClientDbAccess.Route_SimpleUser_GetSessionData.route))]
    [HttpGet(ClientSessionData.Session_GetSessionData_Route)]
    public async Task<ClientSessionData.RawData> GetSessionData_Async() {
        if( !this.SessData.IsLoaded ) {
            throw new NullReferenceException( "Session not loaded." );
        }

        return new ClientSessionData.RawData(
            sessionId: this.SessData.SessionId!,
            userData: this.SessData.User?.GetClientOnlyData(),
            favoriteTermIds: this.SessData.FavoriteTermIds
        );
    }
}
