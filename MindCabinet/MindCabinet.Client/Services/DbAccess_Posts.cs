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

    public const string Post_GetByCriteria_Path = "Post";
    public const string Post_GetByCriteria_Route = "GetByCriteria";

    public async Task<IEnumerable<PostObject>> GetPostsByCriteria_Async( GetPostsByCriteriaParams parameters ) {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            ClientDbAccess.Post_GetByCriteria_Path + "/" + ClientDbAccess.Post_GetByCriteria_Route,
            parameters
        );

        msg.EnsureSuccessStatusCode();

        IEnumerable<PostObject>? ret = await msg.Content.ReadFromJsonAsync<IEnumerable<PostObject>>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize IEnumerable<PostEntry>" );
        }

        return ret;
    }
    
    public const string Post_GetCountByCriteria_Path = "Post";
    public const string Post_GetCountByCriteria_Route = "GetCountByCriteria";

    public async Task<int> GetPostCountByCriteria_Async( GetPostsByCriteriaParams parameters ) {
		JsonContent content = JsonContent.Create( parameters, mediaType: null, null );
        
        //HttpResponseMessage msg = await this.Http.PostAsJsonAsync( "Post/GetCountByCriteria", parameters );
        HttpResponseMessage msg = await this.Http.PostAsync(
            requestUri: ClientDbAccess.Post_GetCountByCriteria_Path + "/" + ClientDbAccess.Post_GetCountByCriteria_Route,
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
    
    public const string Post_Create_Path = "Post";
    public const string Post_Create_Route = "Create";

    public async Task<PostObject> CreatePost_Async( CreatePostParams parameters ) {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            requestUri: ClientDbAccess.Post_Create_Path + "/" + ClientDbAccess.Post_Create_Route,
            value: parameters
        );

        msg.EnsureSuccessStatusCode();

        PostObject? ret = await msg.Content.ReadFromJsonAsync<PostObject>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize PostEntry" );
        }

        return ret;
    }
}
