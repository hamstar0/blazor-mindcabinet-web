using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserContext;
using System.Text.Json.Serialization;


namespace MindCabinet.Shared.DataObjects.UserFavoriteTerm;


public partial class UserFavoriteTermObject( SimpleUserObject simpleUser, int favor, TermObject favTerm ) {
	public SimpleUserObject SimpleUser { get; } = simpleUser;

	public int Favor { get; } = favor;

	public TermObject FavTerm { get; } = favTerm;
}
