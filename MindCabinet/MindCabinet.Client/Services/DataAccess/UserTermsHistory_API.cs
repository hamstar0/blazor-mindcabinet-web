using System.Net.Http.Json;
using Microsoft.AspNetCore.SignalR.Client;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserTermHistory;

namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_UserTermsHistory : IClientDataAccess {
    public interface IAPI : IServerDataAccessAPI {
        public const string BaseRoute = "UserTermsHistory";



        public Task<IEnumerable<UserTermHistoryObject.Raw>> GetHistTermsForCurrentUser_Async();


        public class AddHistTermsForCurrentUser_Params {
            //public SimpleUserId SimpleUserId { get; set; }
            public TermId TermId { get; set; }
        }

        public Task AddHistTermsForCurrentUser_Async( AddHistTermsForCurrentUser_Params parameters );
    }
}
