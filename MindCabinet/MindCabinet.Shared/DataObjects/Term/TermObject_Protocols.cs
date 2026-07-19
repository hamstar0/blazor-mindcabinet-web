using System.Text.Json.Serialization;

namespace MindCabinet.Shared.DataObjects.Term;


public partial class TermObject {
    public static Raw CreateRaw(
            TermId id,
            SimpleUserId creator,
            string term,
            string? abbreviation,
            string? description,
            TermId? contextId = null,
            TermId? aliasId = null ) {
        var raw = new Raw {
            Id = id,
            Creator = creator,
            Term = term,
            Abbreviation = abbreviation,
            Description = description,
            ContextId = contextId,
            AliasId = aliasId
        };
        if( !raw.Validate() ) {
            throw new ArgumentException("Invalid term params");
        }
        return raw;
    }

    public class Raw : IRawDataObject { //IHasId<TermId>
        public TermId Id { get; set; } = default;
        public SimpleUserId Creator { get; set; } = default;
        public string Term { get; set; } = "";
        public string? Abbreviation { get; set; } = "";
        public string? Description { get; set; } = "";
        public TermId? ContextId { get; set; } = null;
        public TermId? AliasId { get; set; } = null;

        
        public async Task<TermObject> ToDataObject_Async( Func<TermId, Task<Raw>> termRawFactory ) {
            return new TermObject(
                id: this.Id,
                creator: this.Creator,
                term: this.Term,
                abbreviation: this.Abbreviation,
                description: this.Description,
                context: this.ContextId is not null
                    ? await termRawFactory( this.ContextId.Value )
                    : null,
                alias: this.AliasId is not null
                    ? await termRawFactory( this.AliasId.Value )
                    : null
            );
        }

        public bool Validate() {
            return this.Id != 0
                && TermObject.ValidateTerm(this.Term)
                && (this.Abbreviation is null || TermObject.ValidateTerm(this.Abbreviation));
        }
    }


    public TermObject.Raw ToRaw() {
        return TermObject.CreateRaw(
            id: this.Id,
            creator: this.Creator,
            term: this.Term,
            abbreviation: this.Abbreviation,
            description: this.Description,
            contextId: this.Context?.Id,
            aliasId: this.Alias?.Id
        );
    }
}
