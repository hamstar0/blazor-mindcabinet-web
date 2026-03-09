using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserContext;
using System.Text.Json.Serialization;


namespace MindCabinet.Shared.DataObjects.UserHistoryTerm;


public partial class UserHistoryTermObject( long simpleUserId, DateTime created, TermObject term ) {
	public long SimpleUserId { get; } = simpleUserId;

	public DateTime Created { get; } = created;

	public TermObject Term { get; } = term;
}
