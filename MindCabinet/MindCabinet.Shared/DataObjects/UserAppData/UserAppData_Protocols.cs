using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsContext;
using System.Text.Json.Serialization;


namespace MindCabinet.Shared.DataObjects;


public partial class UserAppDataObject {
    public static Raw CreateRaw(
            SimpleUserId simpleUserId,
            PostsContextId postsContextId ) {
        return new Raw {
            SimpleUserId = simpleUserId,
            PostsContextId = postsContextId
        };
    }

    public class Raw : IRawDataObject {
		public SimpleUserId SimpleUserId { get; set; }
        
		public PostsContextId PostsContextId { get; set; }

        
        public async Task<UserAppDataObject> CreateDataObject_Async(
                    Func<PostsContextId, Task<PostsContextObject>> postsContextFactory ) {
            return new UserAppDataObject(
                simpleUserId: this.SimpleUserId,
                postsContext: await postsContextFactory( this.PostsContextId )
            );
        }
    }
}
