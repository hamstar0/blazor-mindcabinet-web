using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.UserContext;


public partial class UserContextTermEntryObject {
    public class DatabaseEntry {
        public long UserContextId = default;

        public long TermId = default;

        public double Priority = default;

        public bool IsRequired = default;


		public async Task<UserContextTermEntryObject?> CreateUserContextTermEntry_Async(
                    Func<long, Task<TermObject?>> termFactory ) {
            TermObject? term = await termFactory( this.TermId );
            if( term is null ) {
                return null;
            }
            return new UserContextTermEntryObject(
                term: term,
                priority: this.Priority,
                isRequired: this.IsRequired
            );
		}
	}
}
