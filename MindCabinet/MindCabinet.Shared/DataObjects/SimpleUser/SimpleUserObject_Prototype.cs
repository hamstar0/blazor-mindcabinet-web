using MindCabinet.Shared.Utility;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;


namespace MindCabinet.Shared.DataObjects;


public partial class SimpleUserObject : IEquatable<SimpleUserObject> {
	public class Prototype {
		public long? Id;

		public DateTime? Created;

		public string? Name;

		public string? Email;

		public byte[]? PwHash;

		public byte[]? PwSalt;

		public bool? IsValidated;
	}
}
