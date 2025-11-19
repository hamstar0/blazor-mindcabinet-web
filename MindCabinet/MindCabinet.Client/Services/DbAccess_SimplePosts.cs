using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Threading;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;


namespace MindCabinet.Client.Services;



public partial class ClientDbAccess {
    public class GetPostsByCriteriaParams(
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

    public const string SimplePost_GetByCriteria_Path = "SimplePost";
    public const string SimplePost_GetByCriteria_Route = "GetByCriteria";

    public async Task<IEnumerable<SimplePostObject>> GetPostsByCriteria_Async( GetPostsByCriteriaParams parameters ) {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            ClientDbAccess.SimplePost_GetByCriteria_Path + "/" + ClientDbAccess.SimplePost_GetByCriteria_Route,
            parameters
        );

        msg.EnsureSuccessStatusCode();

        IEnumerable<SimplePostObject>? ret = await msg.Content.ReadFromJsonAsync<IEnumerable<SimplePostObject>>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize IEnumerable<PostEntry>" );
        }

        return ret;
    }
    
    public const string SimplePost_GetCountByCriteria_Path = "SimplePost";
    public const string SimplePost_GetCountByCriteria_Route = "GetCountByCriteria";

    public async Task<int> GetPostCountByCriteria_Async( GetPostsByCriteriaParams parameters ) {
		JsonContent content = JsonContent.Create( parameters, mediaType: null, null );
        
        //HttpResponseMessage msg = await this.Http.PostAsJsonAsync( "Post/GetCountByCriteria", parameters );
        HttpResponseMessage msg = await this.Http.PostAsync(
            requestUri: ClientDbAccess.SimplePost_GetCountByCriteria_Path + "/" + ClientDbAccess.SimplePost_GetCountByCriteria_Route,
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


    public class CreatePostParams(
                string body,
                List<TermObject> tags ) {
        public string Body { get; } = body;
        public List<TermObject> Tags { get; } = tags;
    }
    
    public const string SimplePost_Create_Path = "SimplePost";
    public const string SimplePost_Create_Route = "Create";

    public async Task<SimplePostObject> CreatePost_Async( CreatePostParams parameters ) {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            requestUri: ClientDbAccess.SimplePost_Create_Path + "/" + ClientDbAccess.SimplePost_Create_Route,
            value: parameters
        );

        msg.EnsureSuccessStatusCode();

        SimplePostObject? ret = await msg.Content.ReadFromJsonAsync<SimplePostObject>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize PostEntry" );
        }

        return ret;
    }
}
