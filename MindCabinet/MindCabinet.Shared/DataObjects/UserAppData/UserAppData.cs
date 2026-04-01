using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsContext;
using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects;


namespace MindCabinet.Shared.DataObjects;


public partial class UserAppDataObject : IDataObject {
	public SimpleUserId SimpleUserId { get; private set; }

	public PostsContextObject PostsContext { get; private set; }



	public UserAppDataObject( SimpleUserId simpleUserId, PostsContextObject postsContext ) {
		if( simpleUserId == 0 ) {
			throw new ArgumentException( "SimpleUserId cannot be 0 in UserAppDataObject." );
		}

		this.SimpleUserId = simpleUserId;
		this.PostsContext = postsContext;
	}


	public void SetPostsContext( PostsContextObject context ) {	// i hate this
		this.PostsContext = context;
	}

	public UserAppDataObject.Raw ToRaw() {
		return UserAppDataObject.CreateRaw(
			simpleUserId: this.SimpleUserId,
			postsContextId: this.PostsContext.Id
		);
	}
}
