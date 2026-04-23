using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Threading;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects.PostsContext;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_UserAppData( HttpClient http ) : IClientDataAccess {
    private HttpClient Http = http;


    public class GetForCurrentUser_Return {
        public UserAppDataObject.Raw? UserAppData { get; set; }
    }

    public const string GetForCurrentUser_Path = "UserAppData";
    public const string GetForCurrentUser_Route = "GetForCurrentUser";

    public async Task<GetForCurrentUser_Return> GetForCurrentUser_Async() {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            requestUri: $"{GetForCurrentUser_Path}/{GetForCurrentUser_Route}",
            value: new object()
        );

        msg.EnsureSuccessStatusCode();

        GetForCurrentUser_Return? ret = await msg.Content.ReadFromJsonAsync<GetForCurrentUser_Return>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize GetById_Return" );
        }

        return ret;
    }


    public const string UpdateForCurrentUser_Path = "UserAppData";
    public const string UpdateForCurrentUser_Route = "UpdateForCurrentUser_Route";

    public async Task UpdateForCurrentUser_Async( UserAppDataObject.Prototype parameters ) {
        if( parameters.SimpleUserId is null || parameters.SimpleUserId == 0 ) {
            throw new InvalidOperationException( "No user specified." );
        }

        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            requestUri: $"{UpdateForCurrentUser_Path}/{UpdateForCurrentUser_Route}",
            value: parameters
        );

        msg.EnsureSuccessStatusCode();

        object? ret = await msg.Content.ReadFromJsonAsync<object>();
        if( ret is null ) {
            throw new InvalidDataException( "Maybe something went wrong?" );
        }
    }
}
