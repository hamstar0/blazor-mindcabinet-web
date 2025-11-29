using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Threading;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDbAccess_SimplePosts : IClientDbAccess {
    private HttpClient Http;


    internal ClientDbAccess_SimplePosts( HttpClient http ) {
        this.Http = http;
    }


    public class GetByCriteria_Params(
                string bodyPattern,
                ISet<TermObject> tags,
                bool sortAscendingByDate,
                int pageNumber,
                int postsPerPage ) {
        public string BodyPattern { get; } = bodyPattern;
        public ISet<TermObject> Tags { get; } = tags;
        public bool SortAscendingByDate { get; } = sortAscendingByDate;
        public int PageNumber { get; } = pageNumber;
        public int PostsPerPage { get; } = postsPerPage;


        public override string ToString() {
            return $"[\"{this.BodyPattern}\", "
                +$"({string.Join(",", this.Tags.Select(t=>t.Term))}), "
                +$"{this.SortAscendingByDate}, "
                +$"{this.PageNumber}, "
                +$"{this.PostsPerPage}]";
        }
    }

    public const string GetByCriteria_Path = "SimplePost";
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
                List<TermObject> tags ) {
        public string Body { get; } = body;
        public List<TermObject> Tags { get; } = tags;
    }
    
    public const string Create_Path = "SimplePost";
    public const string Create_Route = "Create";

    public async Task<SimplePostObject> Create_Async( Create_Params parameters ) {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            requestUri: $"{Create_Path}/{Create_Route}",
            value: parameters
        );

        msg.EnsureSuccessStatusCode();

        SimplePostObject? ret = await msg.Content.ReadFromJsonAsync<SimplePostObject>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize SimplePostEntry" );
        }

        return ret;
    }
}
