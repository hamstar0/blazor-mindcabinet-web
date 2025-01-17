using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Threading;
using MindCabinet.Shared.DataEntries;


namespace MindCabinet.Client.Data;



public partial class ClientDataAccess {
    public class GetPostsByCriteriaParams(
                string bodyPattern,
                ISet<TermEntry> tags,
                bool sortAscendingByDate,
                int pageNumber,
                int postsPerPage ) {
        public string BodyPattern { get; } = bodyPattern;
        public ISet<TermEntry> Tags { get; } = tags;
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
    
    public async Task<IEnumerable<PostEntry>> GetPostsByCriteria_Async( GetPostsByCriteriaParams parameters ) {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync( "Post/GetByCriteria", parameters );

        msg.EnsureSuccessStatusCode();

        IEnumerable<PostEntry>? ret = await msg.Content.ReadFromJsonAsync<IEnumerable<PostEntry>>();
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
                IList<TermEntry> tags ) {
        public string Body { get; } = body;
        public IList<TermEntry> Tags { get; } = tags;
    }
    
    public async Task<PostEntry> CreatePost_Async( CreatePostParams parameters ) {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync( "Post/Create", parameters );

        msg.EnsureSuccessStatusCode();

        PostEntry? ret = await msg.Content.ReadFromJsonAsync<PostEntry>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize PostEntry" );
        }

        return ret;
    }
}
