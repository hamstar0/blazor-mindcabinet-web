using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserPostsContext;
using System.Text.Json.Serialization;


namespace MindCabinet.Shared.DataObjects.UserTermFavorite;


public partial class UserTermFavoriteObject {
    public class Raw : IRawDataObject {
		public SimpleUserId SimpleUserId { get; set; }

		public TermId FavTermId { get; set; }

	    public int Favor { get; set; }

        
        
        public async Task<UserTermFavoriteObject> CreateDataObject_Async(
                    Func<SimpleUserId, Task<SimpleUserObject>> userFactory,
                    Func<TermId, Task<TermObject>> termFactory ) {
            return new UserTermFavoriteObject(
                simpleUser: await userFactory( this.SimpleUserId ),
                favor: this.Favor,
                favTerm: await termFactory( this.FavTermId )
            );
        }

        
        public async Task<ClientObject> CreateClientObject_Async(
                    Func<TermId, Task<TermObject>> termFactory ) {
            return new ClientObject(
                simpleUserId: this.SimpleUserId,
                favor: this.Favor,
                favTerm: await termFactory( this.FavTermId )
            );
        }
    }


    
    public class ClientObject( SimpleUserId simpleUserId, int favor, TermObject favTerm ) {
        public SimpleUserId SimpleUserId { get; } = simpleUserId;

        public int Favor { get; } = favor;

        public TermObject FavTerm { get; } = favTerm;
    }
}
