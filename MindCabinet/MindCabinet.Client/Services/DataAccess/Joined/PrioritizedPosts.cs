using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Threading;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Client.Services.DataAccess;


namespace MindCabinet.Client.Services.DbAccess.Joined;



public partial class ClientDataAccess_PrioritizedPosts( HttpClient http ) : IClientDataAccess {
    private HttpClient Http = http;


    public class GetByCriteria_Params(
                string? bodyPattern,
                TermObject[] allTags,
                TermObject[] anyTags,
                bool sortAscendingByDate,
                int pageNumber,
                int postsPerPage ) {
        public string? BodyPattern { get; } = bodyPattern;
        public TermObject[] AllTags { get; } = allTags;
        public TermObject[] AnyTags { get; } = anyTags;
        public bool SortAscendingByDate { get; } = sortAscendingByDate;
        public int PageNumber { get; } = pageNumber;
        public int PostsPerPage { get; } = postsPerPage;


        public override string ToString() {
            return "Prioritized Posts Params: "
                +((this.BodyPattern is not null) ? $"[\"{this.BodyPattern}\", " : "")
                +"["+string.Join(",", this.AllTags.Select(t=>t.Term))+"], "
                +"["+string.Join(",", this.AnyTags.Select(t=>t.Term))+"], "
                +this.SortAscendingByDate+", "
                +this.PageNumber+", "
                +this.PostsPerPage;
        }
    }

    public const string GetByCriteria_Path = "PrioritizedPosts";
    public const string GetByCriteria_Route = "GetByCriteria";

    public async Task<IEnumerable<SimplePostObject>> GetByCriteria_Async( GetByCriteria_Params parameters ) {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            $"{GetByCriteria_Path}/{GetByCriteria_Route}",
            parameters
        );

        msg.EnsureSuccessStatusCode();

        IEnumerable<SimplePostObject>? ret = await msg.Content.ReadFromJsonAsync<IEnumerable<SimplePostObject>>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize IEnumerable<SimplePostObject>" );
        }

        return ret;
    }
}
