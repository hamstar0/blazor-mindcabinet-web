using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects;

namespace MindCabinet.Shared.DataObjects.PostsContext;



public partial class PostsContextOwnersObject : IDataObject {
    public PostsContextObject PostsContext { get; }

    public SimpleUserObject[] Owners { get; }



    public PostsContextOwnersObject(
            PostsContextObject postsContext,
            SimpleUserObject[] owners ) {
        this.PostsContext = postsContext;
        this.Owners = owners;
    }


    public override string ToString() {
		return $"Context \"{this.PostsContext.Name}\" has users "
            +$"{string.Join(", ", this.Owners.Select(u => u.Name))}";
    }
}
