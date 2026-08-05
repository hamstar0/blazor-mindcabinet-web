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



        public class EditForCurrentUser_Params {
            public TermId[] TermIds { get; set; } = [];
            public int[] TermFavors { get; set; } = [];
        }
        public class RemoveForCurrentUser_Params {
            public TermId[] TermIds { get; set; } = [];
        }

        public Task<object> AddTermsForCurrentUser_Async( EditForCurrentUser_Params parameters );

        public Task<object> RemoveTermsForCurrentUser_Async( RemoveForCurrentUser_Params parameters );
        
        public Task<object> UpdateTermsForCurrentUser_Async( EditForCurrentUser_Params parameters );



        public class IncrementFavorForTerm_Params {
            public TermId TermId { get; set; }
        }
        public Task<bool> IncrementFavorForTerm_Async( IncrementFavorForTerm_Params parameters );
    }
}
