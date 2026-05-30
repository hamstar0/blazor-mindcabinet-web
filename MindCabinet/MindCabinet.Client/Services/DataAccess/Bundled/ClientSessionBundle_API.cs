using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.PostsContext;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.Utility;
using System.Net.Http.Json;
using System.Text.Json;

namespace MindCabinet.Client.Services.DbAccess.Bundled;


public partial class ClientDataAccess_ClientSessionBundle : IClientDataAccess {
    public interface IAPI : IServerDataAccessAPI {
        public const string BaseRoute = "Session";



        public class GetCurrentDataBundle_Return {
            public string SessionId { get; set; } = "";

            public SimpleUserObject.ClientObject? UserData { get; set; }

            public UserAppDataObject.Raw? UserAppData { get; set; }

            public PostsContextObject.Raw? UserAppData_PostsContext { get; set; }

            public TermObject.Raw? UserAppData_UserDefaultTerm { get; set; }
        }

        public Task<GetCurrentDataBundle_Return> GetCurrent_Async(
                    object _ );
    }
}
