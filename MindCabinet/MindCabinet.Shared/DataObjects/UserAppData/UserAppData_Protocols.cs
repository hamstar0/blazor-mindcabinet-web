using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsContext;
using System.Text.Json.Serialization;


namespace MindCabinet.Shared.DataObjects;


public partial class UserAppDataObject {
    public static Raw CreateRaw(
            SimpleUserId simpleUserId,
            PostsContextId postsContextId,
            TermId userDefaultTermId ) {
        return new Raw {
            SimpleUserId = simpleUserId,
            PostsContextId = postsContextId,
            UserDefaultTermId = userDefaultTermId
        };
    }

    public class Raw : IRawDataObject {
		public SimpleUserId SimpleUserId { get; set; }
        
		public PostsContextId PostsContextId { get; set; }

		public TermId UserDefaultTermId { get; set; }

        
        public async Task<UserAppDataObject> ToDataObject_Async(
                    Func<PostsContextId, Task<PostsContextObject>> postsContextFactory,
                    Func<TermId, Task<TermObject>> termsFactory ) {
            return new UserAppDataObject(
                simpleUserId: this.SimpleUserId,
                postsContext: await postsContextFactory( this.PostsContextId ),
                userDefaultTerm: await termsFactory( this.UserDefaultTermId )
            );
        }
    }
}
