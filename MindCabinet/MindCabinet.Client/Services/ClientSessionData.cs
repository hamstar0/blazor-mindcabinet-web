using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using System.Net.Http.Json;

namespace MindCabinet.Client.Services;


public partial class ClientSessionData( HttpClient http, ClientDataAccess_UserContext userContextData ) {
    public class RawServerData(
                string sessionId,
                SimpleUserObject.ClientData? userData ) {
        public string SessionId = sessionId;
        public SimpleUserObject.ClientData? UserData = userData;
    }



    private HttpClient Http = http;

    private ClientDataAccess_UserContext UserContextData = userContextData;
    

    public bool IsLoaded { get; private set; } = false;


    private RawServerData? ServerData;

    public string? SessionId { get => this.ServerData?.SessionId; }

    public string? UserName { get => this.ServerData?.UserData?.Name; }



    public const string Get_Path = "Session";
    public const string Get_Route = "Get";

    internal async Task Load_Async() {
        //ClientSessionData.Json? data = await this.Http.GetFromJsonAsync<ClientSessionData.Json>( "Session/Data" );
        HttpResponseMessage msg = await this.Http.GetAsync(
            $"{Get_Path}/{Get_Route}"
        );

        msg.EnsureSuccessStatusCode();

        ClientSessionData.RawServerData? data = await msg.Content.ReadFromJsonAsync<ClientSessionData.RawServerData>();
        if( data is null ) {
            throw new InvalidDataException( "Could not deserialize ClientSessionData.Json" );
        }

        this.ServerData = data;

        this.IsLoaded = true;
    }


    public void Login( SimpleUserObject.ClientData user ) {
        if( this.ServerData is not null ) {
            this.ServerData.UserData = user;
        }
    }

    public void Logout() {
        if( this.ServerData is not null ) {
            this.ServerData.UserData = null;
        }
    }
}
