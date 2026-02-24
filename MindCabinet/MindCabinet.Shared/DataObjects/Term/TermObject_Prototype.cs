using System.Text.Json.Serialization;

namespace MindCabinet.Shared.DataObjects.Term;


public partial class TermObject : IEquatable<TermObject>, IComparable, IComparable<TermObject> {
    public class Prototype {
        public long? Id;

        public string? Term;

        public IdDataObject<TermObject>? Context;

        public IdDataObject<TermObject>? Alias;



        public override string ToString() {
            string ctx = this.Context is not null
                ? this.Context!.Data is not null
                    ? " ("+this.Context.Data!.Term+")"
                    : " {"+this.Context.Id+"}"
                : "";

		    return this.Term ?? ctx;
        }
    }
}
