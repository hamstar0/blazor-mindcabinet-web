using System.Net.Http.Json;
using System.Text.Json;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.PostsQuery;
using MindCabinet.Shared.DataObjects.Term;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_PostsQuery {
    public interface IAPI : IServerDataAccessAPI {
        public const string BaseRoute = "PostsQuery";



        public class Get_Return {
            public IEnumerable<PostsQueryObject.Raw> Queries { get; set; } = [];
        }

        public class GetByCriteria_Params {
            public string? NameContains { get; set; }

            public PostsQueryId[] Ids { get; set; } = [];

            public TermId[] TagTermIds { get; set; } = [];
        }

        public Task<Get_Return> GetForCurrentUserByCriteria_Async(
            GetByCriteria_Params parameters
        );



        public class CreateOrUpdate_Return {
            public PostsQueryId Id { get; set; }
        }
        
        public Task<CreateOrUpdate_Return> CreateForCurrentUser_Async(
            PostsQueryObject.Prototype parameters
        );



        public Task<CreateOrUpdate_Return> UpdateForCurrentUser_Async(
            PostsQueryObject.Prototype parameters
        );
    }
}
