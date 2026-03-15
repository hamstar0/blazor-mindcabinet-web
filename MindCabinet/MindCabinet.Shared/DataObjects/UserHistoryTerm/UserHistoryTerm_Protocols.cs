using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserContext;
using System.Text.Json.Serialization;


namespace MindCabinet.Shared.DataObjects.UserHistoryTerm;


public partial class UserHistoryTermObject {
    public class Raw {
		public long SimpleUserId;

		public long TermId;
        
	    public DateTime Created;

        
        
        public async Task<UserHistoryTermObject> CreateDataObject_Async(
                    Func<long, Task<SimpleUserObject>> userFactory,
                    Func<long, Task<TermObject>> termFactory ) {
            return new UserHistoryTermObject(
                simpleUser: await userFactory( this.SimpleUserId ),
                created: this.Created,
                term: await termFactory( this.TermId )
            );
        }

        
        public async Task<ClientObject> CreateClientObject_Async(
                    Func<long, Task<TermObject>> termFactory ) {
            return new ClientObject(
                simpleUserId: this.SimpleUserId,
                created: this.Created,
                term: await termFactory( this.TermId )
            );
        }
    }


    
    public class ClientObject( long simpleUserId, DateTime created, TermObject term ) {
        public long SimpleUserId { get; } = simpleUserId;

        public DateTime Created { get; } = created;

        public TermObject Term { get; } = term;
    }
}
