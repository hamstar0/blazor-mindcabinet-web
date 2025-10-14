using System.Text.Json.Serialization;

namespace MindCabinet.Shared.DataObjects.Term;


public partial class TermObject : IEquatable<TermObject>, IComparable, IComparable<TermObject> {
    public class Prototype {
        public long? Id;

        public string? Term;

        public Prototype? Context;

        public Prototype? Alias;



        public override string ToString() {
		    return this.Context is not null
			    ? $"{this.Term ?? "-"} ({this.Context.Term ?? "-"})"
			    : this.Term ?? "-";
        }
    }
}
