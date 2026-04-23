using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsContext;
using System.Text.Json.Serialization;


namespace MindCabinet.Shared.DataObjects;


public partial class UserAppDataObject {
    public class Prototype : IRawDataObject {
		public SimpleUserId? SimpleUserId { get; set; }
        
		public PostsContextId? PostsContextId { get; set; }


        
        public bool IsValidAsObject( bool ignoreUserId ) {
            if( !ignoreUserId && this.SimpleUserId is null || this.SimpleUserId == 0 ) {
                return false;
            }
            if( this.PostsContextId is null || this.PostsContextId == 0 ) {
                return false;
            }
            return true;
        }
    }
}
