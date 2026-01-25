using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using System.Net.Http.Json;

namespace MindCabinet.Client.Services;


public partial class ClientSessionData( IServiceScopeFactory serviceScopeFactory ) {
    public class SessionDataJson(
                string sessionId,
                SimpleUserObject.ClientData? userData ) {
        public string SessionId = sessionId;
        public SimpleUserObject.ClientData? UserData = userData;
    }



    private readonly IServiceScopeFactory ServiceScopeFactory = serviceScopeFactory;

    public bool IsLoaded { get; private set; } = false;

    private bool IsLoading = false;


    private SessionDataJson? ServerData;



    public const string Get_Path = "Session";
    public const string Get_Route = "Get";

    internal async Task Load_Async() {
        if( this.IsLoaded || this.IsLoading ) {
            return;
        }
        this.IsLoading = true;
        
        using IServiceScope scope = this.ServiceScopeFactory.CreateScope();
        HttpClient? httpClient = scope.ServiceProvider.GetService<HttpClient>();
        if( httpClient is null ) {
            throw new InvalidOperationException( "HttpClient service not available in ClientSessionData." );
        }

        //ClientSessionData.Json? data = await this.Http.GetFromJsonAsync<ClientSessionData.Json>( "Session/Data" );
        HttpResponseMessage msg = await httpClient.GetAsync(
            $"{Get_Path}/{Get_Route}"
        );

        msg.EnsureSuccessStatusCode();

        ClientSessionData.SessionDataJson? data = await msg.Content
            .ReadFromJsonAsync<ClientSessionData.SessionDataJson>();
        if( data is null ) {
            throw new InvalidDataException( "Could not deserialize ClientSessionData.RawServerData" );
        }

        this.ServerData = data;

        this.IsLoading = false;
        this.IsLoaded = true;
    }
}
