using System.Net.Http.Json;
using System.Text.Json;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.PostsContext;
using MindCabinet.Shared.DataObjects.Term;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_PostsContext {
    public interface IAPI : IServerDataAccessAPI {
        public const string BaseRoute = "PostsContext";



        public class Get_Return {
            public IEnumerable<PostsContextObject.Raw> Contexts { get; set; } = [];
        }

        public class GetByCriteria_Params {
            public string? NameContains { get; set; }

            public PostsContextId[] Ids { get; set; } = [];

            public TermId[] TagTermIds { get; set; } = [];
        }

        public Task<Get_Return> GetForCurrentUserByCriteria_Async(
            GetByCriteria_Params parameters
        );



        public class CreateOrUpdate_Return {
            public PostsContextId Id { get; set; }
        }
        
        public Task<CreateOrUpdate_Return> CreateForCurrentUser_Async(
            PostsContextObject.Prototype parameters
        );



        public Task<CreateOrUpdate_Return> UpdateForCurrentUser_Async(
            PostsContextObject.Prototype parameters
        );
    }
}
