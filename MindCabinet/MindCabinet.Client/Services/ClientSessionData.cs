using MindCabinet.Shared.DataEntries;
using System.Net.Http.Json;

namespace MindCabinet.Client.Services;


public class ClientSessionData {
    public class RawData( string sessionId, SimpleUserEntry.ClientData? userData ) {
        public string SessionId = sessionId;
        public SimpleUserEntry.ClientData? UserData = userData;
    }



    private RawData? Data;

    public string? SessionId { get => Data?.SessionId; }

    public string? UserName { get => Data?.UserData?.Name; }

    public bool IsLoaded { get; private set; } = false;



    internal async Task Load_Async( HttpClient client ) {
        //ClientSessionData.Json? data = await client.GetFromJsonAsync<ClientSessionData.Json>( "Session/Data" );
        HttpResponseMessage msg = await client.GetAsync( "SimpleUser/GetSessionData" );

        msg.EnsureSuccessStatusCode();

        ClientSessionData.RawData? data = await msg.Content.ReadFromJsonAsync<ClientSessionData.RawData>();
        if( data is null ) {
            throw new InvalidDataException( "Could not deserialize ClientSessionData.Json" );
        }

        this.Data = data;

        this.IsLoaded = true;
    }


    public void Login( SimpleUserEntry.ClientData user ) {
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
