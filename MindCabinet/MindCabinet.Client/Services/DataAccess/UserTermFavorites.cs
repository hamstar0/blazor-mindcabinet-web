using System.Net.Http.Json;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserTermFavorite;

namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_UserTermFavorites( HttpClient http, ClientSessionData sessionData ) : IClientDataAccess {
    private HttpClient Http = http;

    private ClientSessionData SessionData = sessionData;


    public class GetTermIdsForCurrentUser_Params {   //( long userId )
        //public long UserId { get; } = userId;
    }

    public const string GetFavTermsForCurrentUser_Path = "UserTermFavorites";
    public const string GetFavTermsForCurrentUser_Route = "GetFavoriteTermsForCurrentUser";

    public async Task<IEnumerable<UserTermFavoriteObject.Raw>> GetFavTermsForCurrentUser_Async() {   //( Get_Params parameters ) {
        if( this.SessionData.UserId is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            requestUri: $"{GetFavTermsForCurrentUser_Path}/{GetFavTermsForCurrentUser_Route}",
            value: new GetTermIdsForCurrentUser_Params()    //parameters
        );

        msg.EnsureSuccessStatusCode();

        IEnumerable<UserTermFavoriteObject.Raw>? ret = await msg.Content.ReadFromJsonAsync<IEnumerable<UserTermFavoriteObject.Raw>>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize IEnumerable<UserTermFavoriteObject.DatabaseEntry>" );
        }

        return ret;
    }


    public class AddTermsForCurrentUser_Params( TermId[] termIds ) {
        public TermId[] TermIds { get; } = termIds;
    }

    public const string AddTermsForCurrentUser_Path = "UserTermFavorites";
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


    public class RemoveTermsForCurrentUser_Params( TermId[] termIds ) {
        public TermId[] TermIds { get; } = termIds;
    }

    public const string RemoveTermsForCurrentUser_Path = "UserTermFavorites";
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
