using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserContext;
using System.Text.Json.Serialization;


namespace MindCabinet.Shared.DataObjects;


public partial class UserAppDataObject {
	public class Prototype {
		public long? SimpleUserId;

		public UserContextObject? UserContext;
	}
}
