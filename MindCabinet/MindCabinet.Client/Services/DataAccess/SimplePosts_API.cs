using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Threading;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Client.Services.DataAccess;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_SimplePosts : IClientDataAccess {
    public interface IAPI : IServerDataAccessAPI {
        public const string BaseRoute = "SimplePost";



        public class GetByCriteria_Params {
            public string? BodyPattern { get; set; }
            public TermId[] AllTagIds { get; set; } = [];
            public bool SortAscendingByDate { get; set; }
            public int PageNumber { get; set; }
            public int PostsPerPage { get; set; }


            public override string ToString() {
                return ((this.BodyPattern is not null) ? $"[\"{this.BodyPattern}\", " : "")
                    // +($"({string.Join(",", this.Tags.Select(t=>t.Term))}), ")
                    +($"({string.Join(",", this.AllTagIds)}), ")
                    +($"{this.SortAscendingByDate}, ")
                    +($"{this.PageNumber}, ")
                    +($"{this.PostsPerPage}]");
            }
        }

        public class GetByCriteria_Return {
            public IEnumerable<SimplePostObject.Raw> Posts { get; set; } = [];
        }

        public Task<GetByCriteria_Return> GetByCriteria_Async( GetByCriteria_Params parameters );
        
        
        public Task<int> GetCountByCriteria_Async( GetByCriteria_Params parameters );



        public class Create_Params {
            public string Body { get; set; } = "";
            public TermId[] TermIds { get; set; } = [];
        }

        public Task<SimplePostObject.Raw> Create_Async( Create_Params parameters );
    }
}
