using System.Net.Http.Json;
using MindCabinet.Shared.DataObjects;


namespace MindCabinet.Client.Services;



public partial class ClientDbAccess {
    public class GetSimpleUserFavoriteTagsParams( long userId ) {
        public long UserId { get; } = userId;
    }

    public const string SimpleUser_GetFavoriteTags_Path = "SimpleUser";
    public const string SimpleUser_GetFavoriteTags_Route = "GetFavoriteTags";

    public async Task<IEnumerable<long>> GetSimpleUserFavoriteTags_Async( GetSimpleUserFavoriteTagsParams parameters ) {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            ClientDbAccess.SimpleUser_GetFavoriteTags_Path + "/" + ClientDbAccess.SimpleUser_GetFavoriteTags_Route,
            parameters
        );

        msg.EnsureSuccessStatusCode();

        IEnumerable<long>? ret = await msg.Content.ReadFromJsonAsync<IEnumerable<long>>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize IEnumerable<long>" );
        }

        return ret;
    }


    public class AddSimpleUserFavoriteTagsParams(
                long userId,
                List<long> termIds ) {
        public long UserId { get; } = userId;
        public List<long> TermIds { get; } = termIds;
    }

    public const string SimpleUser_AddFavoriteTags_Path = "SimpleUser";
    public const string SimpleUser_AddFavoriteTags_Route = "AddFavoriteTags";

    public async Task AddSimpleUserFavoriteTags_Async( AddSimpleUserFavoriteTagsParams parameters ) {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            ClientDbAccess.SimpleUser_AddFavoriteTags_Path + "/" + ClientDbAccess.SimpleUser_AddFavoriteTags_Route,
            parameters
        );

        msg.EnsureSuccessStatusCode();
    }
}
