using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using MindCabinet.Client.Services;
using MindCabinet.Data;
using MindCabinet.Data.DataAccess;
using MindCabinet.Shared.DataObjects;
using System.Data;
using System.Security.Cryptography;
using System.Text;


namespace MindCabinet.Controllers;


[ApiController]
[Route("[controller]")]
public class SessionController(
            ILogger<SessionController> logger,
            ServerDataAccess_Terms termsData,
            ServerSessionData sessData
        ) : ControllerBase {
    private readonly ILogger<SessionController> Logger = logger;

    private readonly ServerDataAccess_Terms TermsData = termsData;
    private readonly ServerSessionData SessData = sessData;

    

    // [HttpPost(nameof(ClientDbAccess.Route_SimpleUser_GetSessionData.route))]
    [HttpPost(ClientSessionData.Get_Route)]
    public async Task<ClientSessionData.SessionDataJson> Get_Async( object parameters ) {
        if( !this.SessData.IsLoaded ) {
            throw new NullReferenceException( "Session not loaded." );
        }

        return new ClientSessionData.SessionDataJson {
            SessionId = this.SessData.SessionId!,
            UserData = this.SessData.User?.GetClientOnlyData()
        };
    }
}
