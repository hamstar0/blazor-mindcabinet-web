using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserPostsContext;
using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects;


namespace MindCabinet.Shared.DataObjects.UserFavoriteTerm;


public partial class UserFavoriteTermObject( SimpleUserObject simpleUser, int favor, TermObject favTerm ) : IDataObject {
	public SimpleUserObject SimpleUser { get; } = simpleUser;

	public int Favor { get; } = favor;

	public TermObject FavTerm { get; } = favTerm;
}
