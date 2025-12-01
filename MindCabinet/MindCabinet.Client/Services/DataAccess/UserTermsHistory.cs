using System.Net.Http.Json;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_UserTermsHistory : IClientDataAccess {
    private HttpClient Http;


    internal ClientDataAccess_UserTermsHistory( HttpClient http ) {
        this.Http = http;
    }


    public class GetByUserId_Params( long userId ) {
        public long UserId { get; } = userId;
    }

    public class GetByUserId_Return( long termId, DateTime created ) {
        public long TermId { get; } = termId;
        public DateTime Created { get; } = created;
    }

    public const string GetByUserId_Path = "UserTermsHistory";
    public const string GetByUserId_Route = "Get";

    public async Task<IEnumerable<GetByUserId_Return>> GetByUserId_Async(
                GetByUserId_Params parameters ) {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            $"{GetByUserId_Path}/{GetByUserId_Route}",
            parameters
        );

        msg.EnsureSuccessStatusCode();

        IEnumerable<GetByUserId_Return>? ret = await msg.Content.ReadFromJsonAsync<IEnumerable<GetByUserId_Return>>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize IEnumerable<ClientDataAccess_UserTermsHistory.GetByUserId_Return>" );
        }

        return ret;
    }


    public class Add_Params(
                //long simpleUserId,
                long termId ) {
        //public long SimpleUserId { get; } = simpleUserId;
        public long TermId { get; } = termId;
    }

    public const string Add_Path = "UserTermsHistory";
    public const string Add_Route = "Add";

    public async Task Add_Async( Add_Params parameters ) {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            $"{Add_Path}/{Add_Route}",
            parameters
        );

        msg.EnsureSuccessStatusCode();
    }
}
