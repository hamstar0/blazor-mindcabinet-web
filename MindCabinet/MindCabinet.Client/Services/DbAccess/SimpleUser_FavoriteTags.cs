using System.Net.Http.Json;
using MindCabinet.Shared.DataObjects;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDbAccess_SimplePosts_FavoriteTags : IClientDbAccess {
    private HttpClient Http;


    internal ClientDbAccess_SimplePosts_FavoriteTags( HttpClient http ) {
        this.Http = http;
    }


    public class Get_Params( long userId ) {
        public long UserId { get; } = userId;
    }

    public const string Get_Path = "SimpleUser";
    public const string Get_Route = "GetFavoriteTagIds";

    public async Task<IEnumerable<long>> Get_Async(
                Get_Params parameters ) {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            $"{Get_Path}/{Get_Route}",
            parameters
        );

        msg.EnsureSuccessStatusCode();

        IEnumerable<long>? ret = await msg.Content.ReadFromJsonAsync<IEnumerable<long>>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize IEnumerable<long>" );
        }

        return ret;
    }


    public class Add_Params(
                long userId,
                List<long> termIds ) {
        public long UserId { get; } = userId;
        public List<long> TermIds { get; } = termIds;
    }

    public const string Add_Path = "SimpleUser";
    public const string Add_Route = "Add";

    public async Task Add_Async( Add_Params parameters ) {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            $"{Add_Path}/{Add_Route}",
            parameters
        );

        msg.EnsureSuccessStatusCode();
    }
}
