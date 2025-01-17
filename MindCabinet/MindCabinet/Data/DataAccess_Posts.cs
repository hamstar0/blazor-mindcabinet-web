using MindCabinet.Client.Data;
using MindCabinet.Shared.DataEntries;


namespace MindCabinet.Data;


public partial class ServerDataAccess {
    private long CurrentPostId = 0;
    private IDictionary<long, PostEntry> Posts = new Dictionary<long, PostEntry> {
        { 4, new PostEntry(
            id: 1,
            timestamp: 4,
            body: "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            tags: new List<TermEntry> { new TermEntry(1, "term1", null, null), new TermEntry(3, "term3", null, null ) }
        ) },
        { 6, new PostEntry(
            id: 2,
            timestamp: 6,
            body: "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
            tags: new List<TermEntry> { new TermEntry(1, "term1", null, null) }
        ) },
        { 7, new PostEntry(
            id: 3,
            timestamp: 7,
            body: "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.",
            tags: new List<TermEntry> { new TermEntry(1, "term1", null, null) }
        ) },
        { 11, new PostEntry(
            id: 4,
            timestamp: 11,
            body: "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
            tags: new List<TermEntry> { new TermEntry(1, "term1", null, null) }
        ) },
        { 14, new PostEntry(
            id: 5,
            timestamp: 14,
            body: "Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo.",
            tags: new List<TermEntry> { new TermEntry(1, "term1", null, null) }
        ) },
        { 18, new PostEntry(
            id: 6,
            timestamp: 18,
            body: "Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt.",
            tags: new List<TermEntry> { new TermEntry(1, "term1", null, null) }
        ) },
        { 22, new PostEntry(
            id: 7,
            timestamp: 22,
            body: "Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem.",
            tags: new List<TermEntry> { new TermEntry(1, "term1", null, null) }
        ) },
        { 31, new PostEntry(
            id: 8,
            timestamp: 31,
            body: "Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur?",
            tags: new List<TermEntry> { new TermEntry(2, "term2", null, null) }
        ) },
        { 32, new PostEntry(
            id: 8,
            timestamp: 32,
            body: "Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur?",
            tags: new List<TermEntry> { new TermEntry(2, "term2", null, null) }
        ) },
        { 36, new PostEntry(
            id: 9,
            timestamp: 36,
            body: "At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis praesentium voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi sint occaecati cupiditate non provident, similique sunt in culpa qui officia deserunt mollitia animi, id est laborum et dolorum fuga.",
            tags: new List<TermEntry> { new TermEntry(2, "term2", null, null), new TermEntry(3, "term3", null, null ) }
        ) },
        { 39, new PostEntry(
            id: 10,
            timestamp: 39,
            body: "Et harum quidem rerum facilis est et expedita distinctio.",
            tags: new List<TermEntry> { new TermEntry(2, "term2", null, null), new TermEntry(3, "term3", null, null ) }
        ) },
        { 44, new PostEntry(
            id: 11,
            timestamp: 44,
            body: "Nam libero tempore, cum soluta nobis est eligendi optio cumque nihil impedit quo minus id quod maxime placeat facere possimus, omnis voluptas assumenda est, omnis dolor repellendus.",
            tags: new List<TermEntry> { new TermEntry(2, "term2", null, null), new TermEntry(3, "term3", null, null ) }
        ) },
        { 49, new PostEntry(
            id: 12,
            timestamp: 49,
            body: "Temporibus autem quibusdam et aut officiis debitis aut rerum necessitatibus saepe eveniet ut et voluptates repudiandae sint et molestiae non recusandae.",
            tags: new List<TermEntry> { new TermEntry(2, "term2", null, null), new TermEntry(3, "term3", null, null ) }
        ) },
        { 55, new PostEntry(
            id: 13,
            timestamp: 55,
            body: "Itaque earum rerum hic tenetur a sapiente delectus, ut aut reiciendis voluptatibus maiores alias consequatur aut perferendis doloribus asperiores repellat.",
            tags: new List<TermEntry> { new TermEntry(2, "term2", null, null), new TermEntry(3, "term3", null, null) }
        ) }
    };



    public async Task<IEnumerable<PostEntry>> GetPostsByCriteria_Async(
                ClientDataAccess.GetPostsByCriteriaParams parameters ) {
        if( parameters.PostsPerPage == 0 ) {
            return Enumerable.Empty<PostEntry>();
        }

        var filteredPosts = this.Posts.Values
			.Where( p => p.Test(parameters.BodyPattern, parameters.Tags) );
		var orderedPosts = filteredPosts
			.OrderBy( p => parameters.SortAscendingByDate ? p.Timestamp : -p.Timestamp );

        IEnumerable<PostEntry> posts = orderedPosts;
        
        if( parameters.PostsPerPage > 0 ) {
            posts = orderedPosts
                .Skip( parameters.PageNumber * parameters.PostsPerPage )
                .Take( parameters.PostsPerPage );
        }

		return posts;
	}

    public async Task<int> GetPostCountByCriteria_Async(
                ClientDataAccess.GetPostsByCriteriaParams parameters ) {
        if( parameters.PostsPerPage == 0 ) {
            return 0;
        }

        var filteredPosts = this.Posts.Values
            .Where( p => p.Test( parameters.BodyPattern, parameters.Tags ) );

        IEnumerable<PostEntry> posts = filteredPosts;

        if( parameters.PostsPerPage > 0 ) {
            posts = filteredPosts
                .Skip( parameters.PageNumber * parameters.PostsPerPage )
                .Take( parameters.PostsPerPage );
        }

        return posts.Count();
    }


	public async Task<PostEntry> CreatePost_Async( ClientDataAccess.CreatePostParams parameters ) {
        long id = this.CurrentPostId++;
		var post = new PostEntry(
			id: id,
			timestamp: DateTime.Now.Ticks,
			body: parameters.Body,
			tags: parameters.Tags
		);

		this.Posts[id] = post;

		return post;
	}
}
