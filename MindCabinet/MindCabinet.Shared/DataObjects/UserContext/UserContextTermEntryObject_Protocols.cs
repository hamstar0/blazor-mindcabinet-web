using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.UserContext;


public partial class UserContextTermEntryObject {
    public class Raw : IRawDataObject {
        public long UserContextId { get; set; } = default;

        public long TermId { get; set; } = default;

        public double Priority { get; set; } = default;

        public bool IsRequired { get; set; } = default;


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
