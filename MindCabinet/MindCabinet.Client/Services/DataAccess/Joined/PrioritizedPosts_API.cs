using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Threading;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects.PostsContext;
using System.Text.Json;


namespace MindCabinet.Client.Services.DbAccess.Joined;



public partial class ClientDataAccess_PrioritizedPosts : IClientDataAccess {
    public interface IAPI : IServerDataAccessAPI {
        public const string BaseRoute = "PrioritizedPosts";



        public class GetByCriteria_Params(
                    PostsContextId postsContextId,
                    string? bodyPattern,
                    TermId[] additionalTagIds,
                    bool sortAscendingByDate,
                    int pageNumber,
                    int postsPerPage ) {
            public PostsContextId PostsContextId { get; set; } = postsContextId;
            public string? BodyPattern { get; set; } = bodyPattern;
            public TermId[] AdditionalTagIds { get; set; } = additionalTagIds;
            public bool SortAscendingByDate { get; set; } = sortAscendingByDate;
            public int PageNumber { get; set; } = pageNumber;
            public int PostsPerPage { get; set; } = postsPerPage;


            public override string ToString() {
                return "Prioritized Posts Params: "
                    +this.PostsContextId+", "
                    +((this.BodyPattern is not null) ? $"[\"{this.BodyPattern}\", " : "")
                    +"["+string.Join(",", this.AdditionalTagIds)+"], "
                    +this.SortAscendingByDate+", "
                    +this.PageNumber+", "
                    +this.PostsPerPage;
            }
        }

        public Task<IEnumerable<SimplePostObject.Raw>> GetByCriteriaForCurrentUser_Async( GetByCriteria_Params parameters );
        
        public Task<int> GetCountByCriteriaForCurrentUser_Async( GetByCriteria_Params parameters );
    }
}
