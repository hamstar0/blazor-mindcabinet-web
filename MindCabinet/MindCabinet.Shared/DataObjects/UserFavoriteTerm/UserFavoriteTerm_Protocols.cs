using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserContext;
using System.Text.Json.Serialization;


namespace MindCabinet.Shared.DataObjects.UserFavoriteTerm;


public partial class UserFavoriteTermObject {
    public class DatabaseEntry {
		public long SimpleUserId;

	    public int Favor;

		public long FavTermId;

        
        
        public async Task<UserFavoriteTermObject> CreateUserAppDataObject_Async(
                    Func<long, Task<TermObject>> termFactory ) {
            return new UserFavoriteTermObject(
                simpleUserId: this.SimpleUserId,
                favor: this.Favor,
                favTerm: await termFactory( this.FavTermId )
            );
        }
    }
}
