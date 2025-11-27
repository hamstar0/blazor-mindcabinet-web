using System.Net.Http.Json;
using MindCabinet.Shared.DataObjects;


namespace MindCabinet.Client.Services;



public partial class ClientDbAccess {
    public class GetSimpleUserFavoriteTagIdsParams( long userId ) {
        public long UserId { get; } = userId;
    }

    public const string SimpleUser_GetFavoriteTagIds_Path = "SimpleUser";
    public const string SimpleUser_GetFavoriteTagIds_Route = "GetFavoriteTagIds";

    public async Task<IEnumerable<long>> GetSimpleUserFavoriteTagIds_Async(
                GetSimpleUserFavoriteTagIdsParams parameters ) {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            $"{SimpleUser_GetFavoriteTagIds_Path}/{SimpleUser_GetFavoriteTagIds_Route}",
            parameters
        );

        msg.EnsureSuccessStatusCode();

        IEnumerable<long>? ret = await msg.Content.ReadFromJsonAsync<IEnumerable<long>>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize IEnumerable<long>" );
        }

        return ret;
    }


    public class AddSimpleUserFavoriteTagsByIdParams(
                long userId,
                List<long> termIds ) {
        public long UserId { get; } = userId;
        public List<long> TermIds { get; } = termIds;
    }

    public const string SimpleUser_AddFavoriteTagsById_Path = "SimpleUser";
    public const string SimpleUser_AddFavoriteTagsById_Route = "AddFavoriteTagsById";

    public async Task AddSimpleUserFavoriteTagsById_Async( AddSimpleUserFavoriteTagsByIdParams parameters ) {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            $"{SimpleUser_AddFavoriteTagsById_Path}/{SimpleUser_AddFavoriteTagsById_Route}",
            parameters
        );

        msg.EnsureSuccessStatusCode();
    }
}
