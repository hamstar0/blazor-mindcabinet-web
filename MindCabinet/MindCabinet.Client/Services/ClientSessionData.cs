using MindCabinet.Shared.DataEntries;
using System.Net.Http.Json;

namespace MindCabinet.Client.Services;


public class ClientSessionData {
    public class JsonData( string sessionId ) {
        public string SessionId = sessionId;
    }



    public JsonData Data { get; private set; } = null!;

    public bool IsLoaded { get; private set; } = false;



    internal async Task Load_Async( HttpClient client ) {
        //ClientSessionData.Json? data = await client.GetFromJsonAsync<ClientSessionData.Json>( "Session/Data" );
        HttpResponseMessage msg = await client.GetAsync( "Session/Data" );

        msg.EnsureSuccessStatusCode();

        ClientSessionData.JsonData? data = await msg.Content.ReadFromJsonAsync<ClientSessionData.JsonData>();
        if( data is null ) {
            throw new InvalidDataException( "Could not deserialize ClientSessionData.Json" );
        }

        this.Data = data;

        this.IsLoaded = true;
    }
}
