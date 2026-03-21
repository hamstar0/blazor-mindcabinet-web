using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserContext;
using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects;


namespace MindCabinet.Shared.DataObjects.UserHistoryTerm;


public partial class UserHistoryTermObject( SimpleUserObject simpleUser, DateTime created, TermObject term ) : IDataObject {
	public SimpleUserObject SimpleUser { get; } = simpleUser;

	public DateTime Created { get; } = created;

	public TermObject Term { get; } = term;
}
