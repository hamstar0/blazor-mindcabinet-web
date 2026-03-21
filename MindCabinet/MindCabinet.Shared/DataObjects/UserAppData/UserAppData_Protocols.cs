using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserContext;
using System.Text.Json.Serialization;


namespace MindCabinet.Shared.DataObjects;


public partial class UserAppDataObject {
    public class Raw : IRawDataObject {
		public long SimpleUserId { get; set; }
        
		public long UserContextId { get; set; }

        
        
        public async Task<UserAppDataObject> CreateDataObject_Async(
                    Func<long, Task<UserContextObject>> userContextFactory ) {
            return new UserAppDataObject(
                simpleUserId: this.SimpleUserId,
                userContext: await userContextFactory( this.UserContextId )
            );
        }
    }
}
