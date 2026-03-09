using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserContext;
using System.Text.Json.Serialization;


namespace MindCabinet.Shared.DataObjects.UserFavoriteTerm;


public partial class UserFavoriteTermObject( long simpleUserId, int favor, TermObject favTerm ) {
	public long SimpleUserId { get; } = simpleUserId;

	public int Favor { get; } = favor;

	public TermObject FavTerm { get; } = favTerm;
}
