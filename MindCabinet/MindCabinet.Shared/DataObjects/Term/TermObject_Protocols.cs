using System.Text.Json.Serialization;

namespace MindCabinet.Shared.DataObjects.Term;


public partial class TermObject : IEquatable<TermObject>, IComparable, IComparable<TermObject> {
    public class Raw : IRawDataObject {
        public long Id { get; set; } = default;
        public string Term { get; set; } = "";
        public long? ContextTermId { get; set; } = null;
        public long? AliasTermId { get; set; } = null;

        
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
