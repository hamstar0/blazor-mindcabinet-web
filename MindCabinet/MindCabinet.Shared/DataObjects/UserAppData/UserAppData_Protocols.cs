using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserPostsContext;
using System.Text.Json.Serialization;


namespace MindCabinet.Shared.DataObjects;


public partial class UserAppDataObject {
    public class Raw : IRawDataObject {
		public SimpleUserId SimpleUserId { get; set; }
        
		public UserPostsContextId UserPostsContextId { get; set; }

        
        
        public async Task<UserAppDataObject> CreateDataObject_Async(
                    Func<UserPostsContextId, Task<UserPostsContextObject>> userPostsContextFactory ) {
            return new UserAppDataObject(
                simpleUserId: this.SimpleUserId,
                userPostsContext: await userPostsContextFactory( this.UserPostsContextId )
            );
        }
    }
}
