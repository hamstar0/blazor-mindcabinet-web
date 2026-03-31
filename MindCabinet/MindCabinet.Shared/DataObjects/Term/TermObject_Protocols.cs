using System.Text.Json.Serialization;

namespace MindCabinet.Shared.DataObjects.Term;


public partial class TermObject {
    public static Raw CreateRaw(
            TermId id,
            string term,
            TermId? contextTermId = null,
            TermId? aliasTermId = null ) {
        return new Raw {
            Id = id,
            Term = term,
            ContextTermId = contextTermId,
            AliasTermId = aliasTermId
        };
    }

    public class Raw : IRawDataObject {
        public TermId Id { get; set; } = default;
        public string Term { get; set; } = "";
        public TermId? ContextTermId { get; set; } = null;
        public TermId? AliasTermId { get; set; } = null;

        
        public async Task<TermObject> CreateDataObject_Async( Func<TermId, Task<Raw>> termRawFactory ) {
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


    public TermObject.Raw ToRaw() {
        return TermObject.CreateRaw(
            id: this.Id,
            term: this.Term,
            contextTermId: this.Context?.Id,
            aliasTermId: this.Alias?.Id
        );
    }
}
