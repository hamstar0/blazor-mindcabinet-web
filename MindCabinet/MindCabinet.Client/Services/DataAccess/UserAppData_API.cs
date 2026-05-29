using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Threading;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects.PostsContext;
using Microsoft.AspNetCore.SignalR.Client;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_UserAppData : IClientDataAccess {
    public interface IAPI : IServerDataAccessAPI {
        public const string BaseRoute = "UserAppData";



        public class GetForCurrentUser_Return {
            public UserAppDataObject.Raw? UserAppData { get; set; }
        }

        public Task<GetForCurrentUser_Return> GetForCurrentUser_Async( object _);



        public Task<object> UpdateForCurrentUser_Async( UserAppDataObject.Prototype parameters );
    }
}
