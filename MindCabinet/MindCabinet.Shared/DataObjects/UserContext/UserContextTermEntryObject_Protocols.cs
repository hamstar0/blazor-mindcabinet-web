using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.UserContext;


public partial class UserContextTermEntryObject {
    public class Raw {
        public long UserContextId = default;

        public long TermId = default;

        public double Priority = default;

        public bool IsRequired = default;


		public async Task<UserContextTermEntryObject> CreateDataObject_Async(
                    Func<long, Task<TermObject>> termFactory ) {
            return new UserContextTermEntryObject(
                term: await termFactory( this.TermId ),
                priority: this.Priority,
                isRequired: this.IsRequired
            );
		}
	}


    public Raw ToRaw( long contextId ) {
        return new Raw {
            UserContextId = contextId,
            TermId = this.Term.Id,
            Priority = this.Priority,
            IsRequired = this.IsRequired
        };
    }
}
