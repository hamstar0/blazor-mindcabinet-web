using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserContext;
using System.Text.Json.Serialization;


namespace MindCabinet.Shared.DataObjects;


public partial class UserAppDataObject( long simpleUserId, UserContextObject userContext ) {
	public long SimpleUserId { get; private set; } = simpleUserId;

	public UserContextObject UserContext { get; private set; } = userContext;


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
