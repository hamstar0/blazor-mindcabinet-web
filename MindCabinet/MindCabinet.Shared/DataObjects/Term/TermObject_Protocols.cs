using System.Text.Json.Serialization;

namespace MindCabinet.Shared.DataObjects.Term;


public partial class TermObject {
    public static Raw CreateRaw(
            TermId id,
            string term,
            TermId? contextId = null,
            TermId? aliasId = null ) {
        return new Raw {
            Id = id,
            Term = term,
            ContextId = contextId,
            AliasId = aliasId
        };
    }

    public class Raw : IRawDataObject {
        public TermId Id { get; set; } = default;
        public string Term { get; set; } = "";
        public TermId? ContextId { get; set; } = null;
        public TermId? AliasId { get; set; } = null;

        
        public async Task<TermObject> ToDataObject_Async( Func<TermId, Task<Raw>> termRawFactory ) {
            return new TermObject(
                id: this.Id,
                term: this.Term,
                context: this.ContextId is not null
                    ? await termRawFactory( this.ContextId.Value )
                    : null,
                alias: this.AliasId is not null
                    ? await termRawFactory( this.AliasId.Value )
                    : null
            );
        }
    }


    public TermObject.Raw ToRaw() {
        return TermObject.CreateRaw(
            id: this.Id,
            term: this.Term,
            contextId: this.Context?.Id,
            aliasId: this.Alias?.Id
        );
    }
}
