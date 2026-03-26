using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.UserContext;


public partial class UserContextTermEntryObject {
    public class Raw : IRawDataObject {
        public UserContextId UserContextId { get; set; } = default;

        public TermId TermId { get; set; } = default;

        public double Priority { get; set; } = default;

        public bool IsRequired { get; set; } = default;


		public async Task<UserContextTermEntryObject> CreateDataObject_Async(
                    Func<TermId, Task<TermObject>> termFactory ) {
            return new UserContextTermEntryObject(
                term: await termFactory( this.TermId ),
                priority: this.Priority,
                isRequired: this.IsRequired
            );
		}
	}


    public Raw ToRaw( UserContextId contextId ) {
        return new Raw {
            UserContextId = contextId,
            TermId = this.Term.Id,
            Priority = this.Priority,
            IsRequired = this.IsRequired
        };
    }
}
