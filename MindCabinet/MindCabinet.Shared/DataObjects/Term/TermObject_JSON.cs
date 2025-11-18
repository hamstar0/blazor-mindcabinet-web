using System.Text.Json.Serialization;

namespace MindCabinet.Shared.DataObjects.Term;


public partial class TermObject : IEquatable<TermObject>, IComparable, IComparable<TermObject> {
    public class JSON {
        public long Id { get; set; }

        public string Term { get; set; }

        public JSON? Context { get; set; }

        public JSON? Alias { get; set; }
    }
}
