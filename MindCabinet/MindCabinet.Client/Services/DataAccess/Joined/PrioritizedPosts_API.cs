using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Threading;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects.PostsContext;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR.Client;


namespace MindCabinet.Client.Services.DbAccess.Joined;



public partial class ClientDataAccess_PrioritizedPosts : IClientDataAccess {
    public interface IAPI : IServerDataAccessAPI {
        public const string BaseRoute = "Terms";



        public class GetByCriteria_Params(
                    PostsContextId postsContextId,
                    string? bodyPattern,
                    TermId[] additionalTagIds,
                    bool sortAscendingByDate,
                    int pageNumber,
                    int postsPerPage ) {
            public PostsContextId PostsContextId { get; } = postsContextId;
            public string? BodyPattern { get; } = bodyPattern;
            public TermId[] AdditionalTagIds { get; } = additionalTagIds;
            public bool SortAscendingByDate { get; } = sortAscendingByDate;
            public int PageNumber { get; } = pageNumber;
            public int PostsPerPage { get; } = postsPerPage;


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

        public Task<IEnumerable<SimplePostObject.Raw>> GetByCriteria_Async( GetByCriteria_Params parameters );

        
        public Task<int> GetCountByCriteria_Async( GetByCriteria_Params parameters );
    }
}
