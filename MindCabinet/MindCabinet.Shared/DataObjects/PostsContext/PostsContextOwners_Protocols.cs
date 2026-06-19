using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects;

namespace MindCabinet.Shared.DataObjects.PostsContext;



public partial class PostsContextOwnersObject : IDataObject {
    public static Raw CreateRaw(
            PostsContextObject postsContext,
            SimpleUserObject user,
            bool isOwner ) {
        return new Raw {
            PostsContextId = postsContext.Id,
            UserId = user.Id,
            IsOwner = isOwner
        };
    }

    

    public class Raw {
        public PostsContextId PostsContextId { get; set; }

        public SimpleUserId UserId { get; set; }

        public bool IsOwner { get; set; }
    }
}
