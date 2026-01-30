using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.UserContext;


public partial class UserContextTermEntryObject {
    public class DatabaseEntry {
        public long TermId = default;
        public double Priority = default;
        public bool IsRequired = default;

		public async Task<UserContextTermEntryObject> CreateUserContextTermEntry_Async( Func<long, Task<TermObject>> termFactory ) {
            return new UserContextTermEntryObject(
                term: await termFactory( this.TermId ),
                priority: this.Priority,
                isRequired: this.IsRequired
            );
		}
	}
}
