using MindCabinet.Shared.DataObjects;
using System.Net.Http.Json;

namespace MindCabinet.Client.Services;


public partial class ClientSessionData {
    public class RawData( string sessionId, SimpleUserObject.ClientData? userData, List<long> favoriteTermIds ) {
        public string SessionId = sessionId;
        public SimpleUserObject.ClientData? UserData = userData;

        public List<long> FavoriteTermIds = favoriteTermIds;
    }



    private HttpClient Http;

    private RawData? Data;

    public string? SessionId { get => Data?.SessionId; }

    public string? UserName { get => Data?.UserData?.Name; }

    public bool IsLoaded { get; private set; } = false;


    public ClientSessionData( HttpClient http ) {
        this.Http = http;
    }



    public const string Session_GetSessionData_Path = "Session";
    public const string Session_GetSessionData_Route = "GetSessionData";

    internal async Task Load_Async() {
        //ClientSessionData.Json? data = await this.Http.GetFromJsonAsync<ClientSessionData.Json>( "Session/Data" );
        HttpResponseMessage msg = await this.Http.GetAsync(
            ClientSessionData.Session_GetSessionData_Path + "/" + ClientSessionData.Session_GetSessionData_Route
        );

        msg.EnsureSuccessStatusCode();

        ClientSessionData.RawData? data = await msg.Content.ReadFromJsonAsync<ClientSessionData.RawData>();
        if( data is null ) {
            throw new InvalidDataException( "Could not deserialize ClientSessionData.Json" );
        }

        this.Data = data;

        this.IsLoaded = true;
    }


    public void Login( SimpleUserObject.ClientData user ) {
        if( this.Data is not null ) {
            this.Data.UserData = user;
        }
    }

    public void Logout() {
        if( this.Data is not null ) {
            this.Data.UserData = null;
        }
    }
}
