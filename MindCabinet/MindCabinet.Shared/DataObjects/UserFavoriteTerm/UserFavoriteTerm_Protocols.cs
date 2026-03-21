using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserContext;
using System.Text.Json.Serialization;


namespace MindCabinet.Shared.DataObjects.UserFavoriteTerm;


public partial class UserFavoriteTermObject {
    public class Raw : IRawDataObject {
		public long SimpleUserId { get; set; }

		public long FavTermId { get; set; }

	    public int Favor { get; set; }

        
        
        public async Task<UserFavoriteTermObject> CreateDataObject_Async(
                    Func<long, Task<SimpleUserObject>> userFactory,
                    Func<long, Task<TermObject>> termFactory ) {
            return new UserFavoriteTermObject(
                simpleUser: await userFactory( this.SimpleUserId ),
                favor: this.Favor,
                favTerm: await termFactory( this.FavTermId )
            );
        }

        
        public async Task<ClientObject> CreateClientObject_Async(
                    Func<long, Task<TermObject>> termFactory ) {
            return new ClientObject(
                simpleUserId: this.SimpleUserId,
                favor: this.Favor,
                favTerm: await termFactory( this.FavTermId )
            );
        }
    }


    
    public class ClientObject( long simpleUserId, int favor, TermObject favTerm ) {
        public long SimpleUserId { get; } = simpleUserId;

        public int Favor { get; } = favor;

        public TermObject FavTerm { get; } = favTerm;
    }
}
