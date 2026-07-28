using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsQuery;
using System.Text.Json.Serialization;


namespace MindCabinet.Shared.DataObjects;


public partial class UserAppDataObject {
    public static Raw CreateRaw(
            SimpleUserId simpleUserId,
            PostsQueryId currentPostsQueryId,
            TermId userDefaultTermId ) {
        return new Raw {
            SimpleUserId = simpleUserId,
            CurrentPostsQueryId = currentPostsQueryId,
            UserDefaultTermId = userDefaultTermId
        };
    }

    public class Raw : IRawDataObject {
		public SimpleUserId SimpleUserId { get; set; }
        
		public PostsQueryId CurrentPostsQueryId { get; set; }

		public TermId UserDefaultTermId { get; set; }

        
        public async Task<UserAppDataObject> ToDataObject_Async(
                    Func<PostsQueryId, Task<PostsQueryObject>> postsQueryFactory,
                    Func<TermId, Task<TermObject>> termsFactory ) {
            return new UserAppDataObject(
                simpleUserId: this.SimpleUserId,
                currentPostsQuery: await postsQueryFactory( this.CurrentPostsQueryId ),
                userDefaultTerm: await termsFactory( this.UserDefaultTermId )
            );
        }
    }
}
