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
    
    public async Task<IEnumerable<PostObject>> GetPostsByCriteria_Async( GetPostsByCriteriaParams parameters ) {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync( "Post/GetByCriteria", parameters );

        msg.EnsureSuccessStatusCode();

        IEnumerable<PostObject>? ret = await msg.Content.ReadFromJsonAsync<IEnumerable<PostObject>>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize IEnumerable<PostEntry>" );
        }

        return ret;
    }
    
    public async Task<int> GetPostCountByCriteria_Async( GetPostsByCriteriaParams parameters ) {
        //HttpResponseMessage msg = await this.Http.PostAsJsonAsync( "Post/GetCountByCriteria", parameters );
		JsonContent content = JsonContent.Create( parameters, mediaType: null, null );
		HttpResponseMessage msg = await this.Http.PostAsync( "Post/GetCountByCriteria", content, default );

		msg.EnsureSuccessStatusCode();

        int? ret = await msg.Content.ReadFromJsonAsync<int>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize int" );
        }

        return ret.Value;
    }


    public class CreatePostParams(
                string body,
                IList<TermObject> tags ) {
        public string Body { get; } = body;
        public IList<TermObject> Tags { get; } = tags;
    }
    
    public async Task<PostObject> CreatePost_Async( CreatePostParams parameters ) {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync( "Post/Create", parameters );

        msg.EnsureSuccessStatusCode();

        PostObject? ret = await msg.Content.ReadFromJsonAsync<PostObject>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize PostEntry" );
        }

        return ret;
    }
}
