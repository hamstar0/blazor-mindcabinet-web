using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsContext;
using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects;

namespace MindCabinet.Shared.DataObjects.UserTermHistory;


public partial class UserTermHistoryObject( SimpleUserObject simpleUser, DateTime created, TermObject term ) : IDataObject {
	public SimpleUserObject SimpleUser { get; } = simpleUser;

	public DateTime Created { get; } = created;

	public TermObject Term { get; } = term;
}
