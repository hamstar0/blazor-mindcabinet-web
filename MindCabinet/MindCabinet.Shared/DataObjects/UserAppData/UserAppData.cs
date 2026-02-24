using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserContext;
using System.Text.Json.Serialization;


namespace MindCabinet.Shared.DataObjects;


public partial class UserAppDataObject( long simpleUserId, IdDataObject<UserContextObject> userContext ) {
	public long SimpleUserId { get; private set; } = simpleUserId;

	public IdDataObject<UserContextObject> UserContext { get; set; } = userContext;
}
