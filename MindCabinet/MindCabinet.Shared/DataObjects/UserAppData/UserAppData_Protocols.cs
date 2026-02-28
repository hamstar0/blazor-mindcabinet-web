using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserContext;
using System.Text.Json.Serialization;


namespace MindCabinet.Shared.DataObjects;


public partial class UserAppDataObject {
    public class DatabaseEntry {
		public long SimpleUserId;
		public long UserContextId;

        
        
        public async Task<UserAppDataObject?> CreateUserAppDataObject_Async(
                    Func<long, Task<UserContextObject>> userContextFactory ) {
            return new UserAppDataObject(
                simpleUserId: this.SimpleUserId,
                userContext: await userContextFactory( this.UserContextId )
            );
        }
    }
}
