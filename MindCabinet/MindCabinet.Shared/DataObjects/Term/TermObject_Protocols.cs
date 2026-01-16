using System.Text.Json.Serialization;

namespace MindCabinet.Shared.DataObjects.Term;


public partial class TermObject : IEquatable<TermObject>, IComparable, IComparable<TermObject> {
    public class DatabaseEntry {
        public long Id = default;
        public string Term = "";
        public long? ContextId = null;
        public long? AliasId = null;
    }
}
