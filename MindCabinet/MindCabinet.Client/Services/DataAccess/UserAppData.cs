using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Threading;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Client.Services.DataAccess;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_UserAppData( HttpClient http ) : IClientDataAccess {
    private HttpClient Http = http;


    public class GetById_Params( long simpleUserId ) {
        public long SimpleUserId { get; } = simpleUserId;
    }

    public class GetById_Return( UserAppDataObject.DatabaseEntry? userAppData ) {
        public UserAppDataObject.DatabaseEntry? UserAppData { get; } = userAppData;
    }

    public const string GetById_Path = "UserAppData";
    public const string GetById_Route = "GetById";

    public async Task<GetById_Return> GetById_Async( GetById_Params parameters ) {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            requestUri: $"{GetById_Path}/{GetById_Route}",
            value: parameters
        );

        msg.EnsureSuccessStatusCode();

        GetById_Return? ret = await msg.Content.ReadFromJsonAsync<GetById_Return>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize GetById_Return" );
        }

        return ret;
    }
}
