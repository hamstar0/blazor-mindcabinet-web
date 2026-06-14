using System.Net.Http.Json;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserTermFavorite;

namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_UserTermFavorites : IClientDataAccess {
    public interface IAPI : IServerDataAccessAPI {
        public const string BaseRoute = "UserTermFavorites";


        public Task<IEnumerable<UserTermFavoriteObject.Raw>> GetFavTermsForCurrentUser_Async( object _ );



        public class AddTermsForCurrentUser_Params {
            public TermId[] TermIds { get; set; } = [];
        }

        public Task AddTermsForCurrentUser_Async( AddTermsForCurrentUser_Params parameters );



        public class RemoveTermsForCurrentUser_Params {
            public TermId[] TermIds { get; set; } = [];
        }

        public Task RemoveTermsForCurrentUser_Async( RemoveTermsForCurrentUser_Params parameters );
    }
}
