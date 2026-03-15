using System.Text.Json.Serialization;

namespace MindCabinet.Shared.DataObjects.Term;


public partial class TermObject : IEquatable<TermObject>, IComparable, IComparable<TermObject> {
    public class Raw {
        public long Id = default;
        public string Term = "";
        public long? ContextTermId = null;
        public long? AliasTermId = null;

        
        public async Task<TermObject> CreateDataObject_Async(
                    Func<long, Task<TermObject.Raw>> termRawFactory ) {
            return new TermObject(
                id: this.Id,
                term: this.Term,
                context: this.ContextTermId is not null
                    ? await termRawFactory( this.ContextTermId.Value )
                    : null,
                alias: this.AliasTermId is not null
                    ? await termRawFactory( this.AliasTermId.Value )
                    : null
            );
        }
    }
}
