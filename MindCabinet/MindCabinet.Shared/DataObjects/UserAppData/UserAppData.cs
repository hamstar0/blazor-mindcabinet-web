using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserPostsContext;
using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects;


namespace MindCabinet.Shared.DataObjects;


public partial class UserAppDataObject : IDataObject {
	public SimpleUserId SimpleUserId { get; private set; }

	public UserPostsContextObject UserPostsContext { get; private set; }



	public UserAppDataObject( SimpleUserId simpleUserId, UserPostsContextObject userPostsContext ) {
		if( simpleUserId == 0 ) {
			throw new ArgumentException( "SimpleUserId cannot be 0 in UserAppDataObject." );
		}

		this.SimpleUserId = simpleUserId;
		this.UserPostsContext = userPostsContext;
	}


	public void SetUserPostsContext( UserPostsContextObject context ) {	// i hate this
		this.UserPostsContext = context;
	}

	public UserAppDataObject.Raw ToRaw() {
		return UserAppDataObject.CreateRaw(
			simpleUserId: this.SimpleUserId,
			userPostsContextId: this.UserPostsContext.Id
		);
	}
}
