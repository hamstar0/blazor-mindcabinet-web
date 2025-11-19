using MindCabinet.Shared.DataObjects.Term;
using System.Text.Json.Serialization;


namespace MindCabinet.Shared.DataObjects;


public partial class SimplePostObject : IEquatable<SimplePostObject> {
	public class Prototype {
		public long? Id;

		public DateTime? Created;

		public string? Body;

		public List<TermObject>? Tags;
	}
}
