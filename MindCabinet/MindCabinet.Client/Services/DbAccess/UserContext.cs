using System.Net.Http.Json;
using MindCabinet.Shared.DataObjects;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDbAccess_UserContext {
    private HttpClient Http;


    internal ClientDbAccess_UserContext( HttpClient http ) {
        this.Http = http;
    }


    public class GetContextsByUserIdParams( long userId ) {
        public long UserId { get; } = userId;
    }

    public const string Context_GetContextsByUserId_Path = "Context";
    public const string Context_GetContextsByUserId_Route = "GetContextsByUserId";

    public async Task<IEnumerable<long>> GetContextsByUserId_Async( GetSimpleUserFavoriteTagIdsParams parameters ) {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            ClientDbAccess.SimpleUser_GetFavoriteTagIds_Path + "/" + ClientDbAccess.SimpleUser_GetFavoriteTagIds_Route,
            parameters
        );
        
        msg.EnsureSuccessStatusCode();
        
        IEnumerable<long>? ret = await msg.Content.ReadFromJsonAsync<IEnumerable<long>>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize IEnumerable<long>" );
        }

        return ret;
    }
}
