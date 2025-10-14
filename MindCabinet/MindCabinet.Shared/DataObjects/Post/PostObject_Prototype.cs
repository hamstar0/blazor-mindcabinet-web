using MindCabinet.Shared.DataObjects.Term;
using System.Text.Json.Serialization;


namespace MindCabinet.Shared.DataObjects;


public partial class PostObject : IEquatable<PostObject> {
	public class Prototype {
		public long? Id;

		public DateTime? Created;

		public string? Body;

		public IList<TermObject>? Tags;
	}
}
