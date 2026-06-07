using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsContext;
using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects;


namespace MindCabinet.Shared.DataObjects;


public partial class UserAppDataObject : IDataObject {
	public SimpleUserId SimpleUserId { get; private set; }

	public PostsContextObject CurrentPostsContext { get; private set; }

	public TermObject UserDefaultTerm { get; private set; }



	public UserAppDataObject(
			SimpleUserId simpleUserId,
			PostsContextObject currentPostsContext,
			TermObject userDefaultTerm ) {
		if( simpleUserId == 0 ) {
			throw new ArgumentException( "SimpleUserId cannot be 0 in UserAppDataObject." );
		}

		this.SimpleUserId = simpleUserId;
		this.CurrentPostsContext = currentPostsContext;
		this.UserDefaultTerm = userDefaultTerm;
	}


	public void SetCurrentPostsContext( PostsContextObject context ) {	// i hate this
		this.CurrentPostsContext = context;
	}

	public UserAppDataObject.Raw ToRaw() {
		return UserAppDataObject.CreateRaw(
			simpleUserId: this.SimpleUserId,
			currentPostsContextId: this.CurrentPostsContext.Id,
			userDefaultTermId: this.UserDefaultTerm.Id
		);
	}
}
