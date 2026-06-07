using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsContext;
using System.Text.Json.Serialization;


namespace MindCabinet.Shared.DataObjects;


public partial class UserAppDataObject {
    public static Raw CreateRaw(
            SimpleUserId simpleUserId,
            PostsContextId currentPostsContextId,
            TermId userDefaultTermId ) {
        return new Raw {
            SimpleUserId = simpleUserId,
            CurrentPostsContextId = currentPostsContextId,
            UserDefaultTermId = userDefaultTermId
        };
    }

    public class Raw : IRawDataObject {
		public SimpleUserId SimpleUserId { get; set; }
        
		public PostsContextId CurrentPostsContextId { get; set; }

		public TermId UserDefaultTermId { get; set; }

        
        public async Task<UserAppDataObject> ToDataObject_Async(
                    Func<PostsContextId, Task<PostsContextObject>> postsContextFactory,
                    Func<TermId, Task<TermObject>> termsFactory ) {
            return new UserAppDataObject(
                simpleUserId: this.SimpleUserId,
                currentPostsContext: await postsContextFactory( this.CurrentPostsContextId ),
                userDefaultTerm: await termsFactory( this.UserDefaultTermId )
            );
        }
    }
}
