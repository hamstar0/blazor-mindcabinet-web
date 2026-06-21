using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects;

namespace MindCabinet.Shared.DataObjects.PostsContext;



public partial class PostsContextOwnersObject : IDataObject {
    public static Raw CreateRaw(
            PostsContextId postsContextId,
            SimpleUserId simpleUserId ) {
        return new Raw {
            PostsContextId = postsContextId,
            SimpleUserId = simpleUserId
        };
    }

    

    public class Raw {
        public PostsContextId PostsContextId { get; set; }

        public SimpleUserId SimpleUserId { get; set; }
    }
}
