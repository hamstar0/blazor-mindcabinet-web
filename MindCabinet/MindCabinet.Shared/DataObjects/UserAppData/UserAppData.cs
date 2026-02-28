using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserContext;
using System.Text.Json.Serialization;


namespace MindCabinet.Shared.DataObjects;


public partial class UserAppDataObject( long simpleUserId, UserContextObject userContext ) {
	public long SimpleUserId { get; private set; } = simpleUserId;

	public UserContextObject UserContext { get; private set; } = userContext;
}
