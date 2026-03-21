using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserContext;
using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects;


namespace MindCabinet.Shared.DataObjects;


public partial class UserAppDataObject : IDataObject {
	public long SimpleUserId { get; private set; }

	public UserContextObject UserContext { get; private set; }



	public UserAppDataObject( long simpleUserId, UserContextObject userContext ) {
		if( simpleUserId == 0 ) {
			throw new ArgumentException( "SimpleUserId cannot be 0 in UserAppDataObject." );
		}

		this.SimpleUserId = simpleUserId;
		this.UserContext = userContext;
	}


	public void SetUserContext( UserContextObject context ) {	// i hate this
		this.UserContext = context;
	}

	public UserAppDataObject.Raw ToRaw() {
		return new UserAppDataObject.Raw() {
			SimpleUserId = this.SimpleUserId,
			UserContextId = this.UserContext.Id
		};
	}
}
