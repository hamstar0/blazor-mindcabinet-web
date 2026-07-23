using System.Text.Json;
using System.Text.Json.Serialization;

namespace MindCabinet.Shared.DataObjects.Term;


public partial class TermObject {
    public static Raw CreateRaw(
            TermId id,
            SimpleUserId creator,
            string term,
            string? abbreviation,
            string? description,
            TermId? contextId,
            TermId? aliasId ) {
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
            throw new ArgumentException( "Invalid term params: "+JsonSerializer.Serialize(raw) );
        }
        return raw;
    }

    public class Raw : IRawDataObject { //IHasId<TermId>
        public TermId Id { get; set; }
        public SimpleUserId Creator { get; set; }
        public string Term { get; set; } = "";
        public string? Abbreviation { get; set; }
        public string? Description { get; set; }
        public TermId? ContextId { get; set; }
        public TermId? AliasId { get; set; }

        
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
                && this.Creator != 0
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
