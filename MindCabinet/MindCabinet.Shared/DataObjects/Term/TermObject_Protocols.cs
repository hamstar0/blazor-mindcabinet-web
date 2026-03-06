using System.Text.Json.Serialization;

namespace MindCabinet.Shared.DataObjects.Term;


public partial class TermObject : IEquatable<TermObject>, IComparable, IComparable<TermObject> {
    public class DatabaseEntry {
        public long Id = default;
        public string Term = "";
        public long? ContextTermId = null;
        public long? AliasTermId = null;

        
        public async Task<TermObject> CreateTermObject_Async(
                    Func<long, Task<TermObject>>? termFactoryIfIncludesContextAndAlias ) {
            if( termFactoryIfIncludesContextAndAlias is null ) {
                return new TermObject(
                    id: this.Id,
                    term: this.Term,
                    contextId: this.ContextTermId,
                    aliasId: this.AliasTermId
                );
            } else {
                return new TermObject(
                    id: this.Id,
                    term: this.Term,
                    context: this.ContextTermId is not null
                        ? await termFactoryIfIncludesContextAndAlias(this.ContextTermId.Value)
                        : null,
                    alias: this.AliasTermId is not null
                        ? await termFactoryIfIncludesContextAndAlias(this.AliasTermId.Value)
                        : null
                );
            }
        }
    }
}
