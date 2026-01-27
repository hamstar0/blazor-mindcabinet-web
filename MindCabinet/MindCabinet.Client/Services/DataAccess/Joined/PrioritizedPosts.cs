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
                long userContextId,
                string? bodyPattern,
                long[] additionalTagIds,
                bool sortAscendingByDate,
                int pageNumber,
                int postsPerPage ) {
        public long UserContextId { get; } = userContextId;
        public string? BodyPattern { get; } = bodyPattern;
        public long[] AdditionalTagIds { get; } = additionalTagIds;
        public bool SortAscendingByDate { get; } = sortAscendingByDate;
        public int PageNumber { get; } = pageNumber;
        public int PostsPerPage { get; } = postsPerPage;


        public override string ToString() {
            return "Prioritized Posts Params: "
                +this.UserContextId+", "
                +((this.BodyPattern is not null) ? $"[\"{this.BodyPattern}\", " : "")
                +"["+string.Join(",", this.AdditionalTagIds)+"], "
                +this.SortAscendingByDate+", "
                +this.PageNumber+", "
                +this.PostsPerPage;
        }
    }

    

    public const string GetByCriteria_Path = "PrioritizedPosts";
    public const string GetByCriteria_Route = "GetByCriteria";

    public async Task<IEnumerable<SimplePostObject>> GetByCriteria_Async( GetByCriteria_Params parameters ) {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            requestUri: $"{GetByCriteria_Path}/{GetByCriteria_Route}",
            value: parameters
        );

        msg.EnsureSuccessStatusCode();

        IEnumerable<SimplePostObject>? ret = await msg.Content.ReadFromJsonAsync<IEnumerable<SimplePostObject>>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize IEnumerable<SimplePostObject>" );
        }

        return ret;
    }
    
    public const string GetCountByCriteria_Path = "PrioritizedPosts";
    public const string GetCountByCriteria_Route = "GetCountByCriteria";

    public async Task<int> GetCountByCriteria_Async( GetByCriteria_Params parameters ) {
		JsonContent content = JsonContent.Create( parameters, mediaType: null, null );
        
        //HttpResponseMessage msg = await this.Http.PostAsJsonAsync( "Post/GetCountByCriteria", parameters );
        HttpResponseMessage msg = await this.Http.PostAsync(
            requestUri: $"{GetCountByCriteria_Path}/{GetCountByCriteria_Route}",
            content: content,
            cancellationToken: default
        );

		msg.EnsureSuccessStatusCode();

        int? ret = await msg.Content.ReadFromJsonAsync<int>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize int" );
        }

        return ret.Value;
    }
}
