using System.Text.Json.Serialization;
using MindCabinet.Shared.Utility;

namespace MindCabinet.Shared.DataObjects.Term;


public partial class TermObject : IEquatable<TermObject>, IComparable, IComparable<TermObject> {
    public class Prototype {
        public long? Id;

        public string? Term;

        public PrimitiveOptional<long>? ContextTermId;

        public PrimitiveOptional<long>? AliasTermId;
    }
}
