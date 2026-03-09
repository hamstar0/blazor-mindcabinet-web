using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserContext;
using System.Text.Json.Serialization;


namespace MindCabinet.Shared.DataObjects.UserHistoryTerm;


public partial class UserHistoryTermObject {
    public class DatabaseEntry {
		public long SimpleUserId;

	    public DateTime Created;

		public long TermId;

        
        
        public async Task<UserHistoryTermObject> CreateUserAppDataObject_Async(
                    Func<long, Task<TermObject>> termFactory ) {
            return new UserHistoryTermObject(
                simpleUserId: this.SimpleUserId,
                created: this.Created,
                term: await termFactory( this.TermId )
            );
        }
    }
}
