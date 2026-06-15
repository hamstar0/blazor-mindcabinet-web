using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.PostsContext;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.Utility;
using System.Net.Http.Json;
using System.Text.Json;

namespace MindCabinet.Client.Services.DbAccess;


public partial class ClientDataAccess_UserSession : IClientDataAccess {
    public interface IAPI : IServerDataAccessAPI {
        public const string BaseRoute = "UserSession";



        public Task<object> Logout_Async( int _ );
    }
}
