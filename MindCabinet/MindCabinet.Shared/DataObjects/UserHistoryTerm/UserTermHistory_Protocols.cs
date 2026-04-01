using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsContext;
using System.Text.Json.Serialization;

namespace MindCabinet.Shared.DataObjects.UserTermHistory;


public partial class UserTermHistoryObject {
    public static Raw CreateRaw(
            SimpleUserId simpleUserId,
            TermId termId,
            DateTime created ) {
        return new Raw {
            SimpleUserId = simpleUserId,
            TermId = termId,
            Created = created
        };
    }

    public class Raw : IRawDataObject {
		public SimpleUserId SimpleUserId { get; set; }

		public TermId TermId { get; set; }
        
	    public DateTime Created { get; set; }

        
        public async Task<UserTermHistoryObject> CreateDataObject_Async(
                    Func<SimpleUserId, Task<SimpleUserObject>> userFactory,
                    Func<TermId, Task<TermObject>> termFactory ) {
            return new UserTermHistoryObject(
                simpleUser: await userFactory( this.SimpleUserId ),
                created: this.Created,
                term: await termFactory( this.TermId )
            );
        }

        
        public async Task<ClientObject> CreateClientObject_Async(
                    Func<TermId, Task<TermObject>> termFactory ) {
            return new ClientObject(
                simpleUserId: this.SimpleUserId,
                created: this.Created,
                term: await termFactory( this.TermId )
            );
        }
    }


    
    public class ClientObject( SimpleUserId simpleUserId, DateTime created, TermObject term ) {
        public SimpleUserId SimpleUserId { get; } = simpleUserId;

        public DateTime Created { get; } = created;

        public TermObject Term { get; } = term;
    }
}
