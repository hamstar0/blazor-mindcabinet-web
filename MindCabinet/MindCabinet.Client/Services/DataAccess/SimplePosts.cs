using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Threading;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Client.Services.DataAccess;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_SimplePosts( HttpClient http ) : IClientDataAccess {
    private HttpClient Http = http;


    public class GetByCriteria_Params(
                string? bodyPattern,
                long[] allTagIds,
                bool sortAscendingByDate,
                int pageNumber,
                int postsPerPage ) {
        public string? BodyPattern { get; } = bodyPattern;
        public long[] AllTagIds { get; } = allTagIds;
        public bool SortAscendingByDate { get; } = sortAscendingByDate;
        public int PageNumber { get; } = pageNumber;
        public int PostsPerPage { get; } = postsPerPage;


        public override string ToString() {
            return ((this.BodyPattern is not null) ? $"[\"{this.BodyPattern}\", " : "")
                // +($"({string.Join(",", this.Tags.Select(t=>t.Term))}), ")
                +($"({string.Join(",", this.AllTagIds)}), ")
                +($"{this.SortAscendingByDate}, ")
                +($"{this.PageNumber}, ")
                +($"{this.PostsPerPage}]");
        }
    }

    public class GetByCriteria_Return( IEnumerable<SimplePostObject.Raw> posts ) {
        public IEnumerable<SimplePostObject.Raw> Posts { get; } = posts;
    }

    public const string GetByCriteria_Path = "SimplePost";
    public const string GetByCriteria_Route = "GetByCriteria";

    public async Task<GetByCriteria_Return> GetByCriteria_Async( GetByCriteria_Params parameters ) {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            requestUri: $"{GetByCriteria_Path}/{GetByCriteria_Route}",
            value: parameters
        );

        msg.EnsureSuccessStatusCode();

        GetByCriteria_Return? ret = await msg.Content.ReadFromJsonAsync<GetByCriteria_Return>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize GetByCriteria_Return" );
        }

        return ret;
    }
    
    public const string GetCountByCriteria_Path = "SimplePost";
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


    public class Create_Params(
                string body,
                TermId[] termIds ) {
        public string Body { get; } = body;
        public TermId[] TermIds { get; } = termIds;
    }
    
    public const string Create_Path = "SimplePost";
    public const string Create_Route = "Create";

    public async Task<SimplePostObject.Raw> Create_Async( Create_Params parameters ) {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            requestUri: $"{Create_Path}/{Create_Route}",
            value: parameters
        );

        msg.EnsureSuccessStatusCode();

        SimplePostObject.Raw? ret = await msg.Content.ReadFromJsonAsync<SimplePostObject.Raw>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize SimplePostEntry" );
        }

        return ret;
    }
}
