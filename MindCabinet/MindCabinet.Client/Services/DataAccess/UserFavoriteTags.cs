using System.Net.Http.Json;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_UserFavoriteTerms( HttpClient http ) : IClientDataAccess {
    private HttpClient Http = http;


    public class Get_Params( long userId ) {
        public long UserId { get; } = userId;
    }

    public const string Get_Path = "UserFavoriteTags";
    public const string Get_Route = "GetIds";

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


    public class AddTerms_Params(
                List<long> termIds ) {
        public List<long> TermIds { get; } = termIds;
    }

    public const string AddTerms_Path = "UserFavoriteTags";
    public const string AddTerms_Route = "AddTerms";

    public async Task AddTerms_Async( AddTerms_Params parameters ) {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            $"{AddTerms_Path}/{AddTerms_Route}",
            parameters
        );

        msg.EnsureSuccessStatusCode();
    }
}
