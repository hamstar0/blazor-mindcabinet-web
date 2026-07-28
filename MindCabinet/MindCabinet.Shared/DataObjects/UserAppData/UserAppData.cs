using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsQuery;
using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects;


namespace MindCabinet.Shared.DataObjects;


public partial class UserAppDataObject : IDataObject {
	public SimpleUserId SimpleUserId { get; private set; }

	public PostsQueryObject CurrentPostsQuery { get; private set; }

	public TermObject UserDefaultTerm { get; private set; }



	public UserAppDataObject(
			SimpleUserId simpleUserId,
			PostsQueryObject currentPostsQuery,
			TermObject userDefaultTerm ) {
		if( simpleUserId == 0 ) {
			throw new ArgumentException( "SimpleUserId cannot be 0 in UserAppDataObject." );
		}

		this.SimpleUserId = simpleUserId;
		this.CurrentPostsQuery = currentPostsQuery;
		this.UserDefaultTerm = userDefaultTerm;
	}


	public void SetCurrentPostsQuery( PostsQueryObject context ) {	// i hate this
		this.CurrentPostsQuery = context;
	}

	public UserAppDataObject.Raw ToRaw() {
		return UserAppDataObject.CreateRaw(
			simpleUserId: this.SimpleUserId,
			currentPostsQueryId: this.CurrentPostsQuery.Id,
			userDefaultTermId: this.UserDefaultTerm.Id
		);
	}
}
