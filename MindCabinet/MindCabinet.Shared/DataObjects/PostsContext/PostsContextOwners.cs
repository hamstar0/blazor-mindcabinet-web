using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects;

namespace MindCabinet.Shared.DataObjects.PostsContext;



public partial class PostsContextOwnersObject : IDataObject {
    public PostsContextObject PostsContext { get; }

    public SimpleUserObject User { get; }

    public bool IsOwner { get; }



    public PostsContextOwnersObject(
            PostsContextObject postsContext,
            SimpleUserObject user,
            bool isOwner ) {
        this.PostsContext = postsContext;
        this.User = user;
        this.IsOwner = isOwner;
    }


    public override string ToString() {
		return $"Context \"{this.PostsContext.Name}\" has user {this.User.Name}"
            +(this.IsOwner ? " (owner)" : "");
    }
}
