using System.Net.Http.Json;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_UserTermsHistory( HttpClient http, ClientSessionData sessionData ) : IClientDataAccess {
    private HttpClient Http = http;

    private ClientSessionData SessionData = sessionData;


    public class GetTermIdsForCurrentUser_Params { //( long userId ) {
        //public long UserId { get; } = userId;
    }

    public class GetTermIdsForCurrentUser_Return( long termId, DateTime created ) {
        public long TermId { get; } = termId;
        public DateTime Created { get; } = created;
    }

    public const string GetTermIdsForCurrentUser_Path = "UserTermsHistory";
    public const string GetTermIdsForCurrentUser_Route = "GetTermIdsForCurrentUser";

    public async Task<IEnumerable<GetTermIdsForCurrentUser_Return>> GetTermIdsForCurrentUser_Async() {
        if( this.SessionData.UserId is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            $"{GetTermIdsForCurrentUser_Path}/{GetTermIdsForCurrentUser_Route}",
            new GetTermIdsForCurrentUser_Params()  //parameters
        );

        msg.EnsureSuccessStatusCode();

        IEnumerable<GetTermIdsForCurrentUser_Return>? ret = await msg.Content.ReadFromJsonAsync<IEnumerable<GetTermIdsForCurrentUser_Return>>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize IEnumerable<ClientDataAccess_UserTermsHistory.GetByUserId_Return>" );
        }

        return ret;
    }


    public class AddTermsForCurrentUser_Params(
                //long simpleUserId,
                long termId ) {
        //public long SimpleUserId { get; } = simpleUserId;
        public long TermId { get; } = termId;
    }

    public const string AddTermsForCurrentUser_Path = "UserTermsHistory";
    public const string AddTermsForCurrentUser_Route = "AddTermsForCurrentUser";

    public async Task AddTermsForCurrentUser_Async( AddTermsForCurrentUser_Params parameters ) {
        if( this.SessionData.UserId is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            $"{AddTermsForCurrentUser_Path}/{AddTermsForCurrentUser_Route}",
            parameters
        );

        msg.EnsureSuccessStatusCode();
    }
}
