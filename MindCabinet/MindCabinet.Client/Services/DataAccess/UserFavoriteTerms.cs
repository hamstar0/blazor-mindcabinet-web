using System.Net.Http.Json;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_UserFavoriteTerms( HttpClient http, ClientSessionData sessionData ) : IClientDataAccess {
    private HttpClient Http = http;

    private ClientSessionData SessionData = sessionData;


    public class GetTermIdsForCurrentUser_Params {   //( long userId )
        //public long UserId { get; } = userId;
    }

    public const string GetTermIdsForCurrentUser_Path = "UserFavoriteTerms";
    public const string GetTermIdsForCurrentUser_Route = "GetTermIdsForCurrentUser";

    public async Task<IEnumerable<long>> GetTermIdsForCurrentUser_Async() {   //( Get_Params parameters ) {
        if( this.SessionData.UserId is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            requestUri: $"{GetTermIdsForCurrentUser_Path}/{GetTermIdsForCurrentUser_Route}",
            value: new GetTermIdsForCurrentUser_Params()    //parameters
        );

        msg.EnsureSuccessStatusCode();

        IEnumerable<long>? ret = await msg.Content.ReadFromJsonAsync<IEnumerable<long>>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize IEnumerable<long>" );
        }

        return ret;
    }


    public class AddTermsForCurrentUser_Params( List<long> termIds ) {
        public List<long> TermIds { get; } = termIds;
    }

    public const string AddTermsForCurrentUser_Path = "UserFavoriteTerms";
    public const string AddTermsForCurrentUser_Route = "AddTermsForCurrentUser";

    public async Task AddTermsForCurrentUser_Async( AddTermsForCurrentUser_Params parameters ) {
        if( this.SessionData.UserId is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            requestUri: $"{AddTermsForCurrentUser_Path}/{AddTermsForCurrentUser_Route}",
            value: parameters
        );

        msg.EnsureSuccessStatusCode();
    }


    public class RemoveTermsForCurrentUser_Params( List<long> termIds ) {
        public List<long> TermIds { get; } = termIds;
    }

    public const string RemoveTermsForCurrentUser_Path = "UserFavoriteTerms";
    public const string RemoveTermsForCurrentUser_Route = "RemoveTermsForCurrentUser";

    public async Task RemoveTermsForCurrentUser_Async( RemoveTermsForCurrentUser_Params parameters ) {
        if( this.SessionData.UserId is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            requestUri: $"{RemoveTermsForCurrentUser_Path}/{RemoveTermsForCurrentUser_Route}",
            value: parameters
        );

        msg.EnsureSuccessStatusCode();
    }
}
