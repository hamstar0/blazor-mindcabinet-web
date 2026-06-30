using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsContext;
using System.Text.Json.Serialization;


namespace MindCabinet.Shared.DataObjects;


public partial class UserAppDataObject {
    public class Prototype : IRawDataObject {
		public SimpleUserId? SimpleUserId { get; set; }
        
		public PostsContextId? CurrentPostsContextId { get; set; }

        public TermId? UserDefaultTermId { get; set; }


        
        public bool IsValidAsObject( bool ignoreUserId, bool ignoreUserTerm ) {
            if( !ignoreUserId && (this.SimpleUserId is null || this.SimpleUserId == 0) ) {
                return false;
            }
            if( this.CurrentPostsContextId is null || this.CurrentPostsContextId == 0 ) {
                return false;
            }
            if( !ignoreUserTerm && (this.UserDefaultTermId is null || this.UserDefaultTermId == 0) ) {
                return false;
            }
            return true;
        }
    }
}
