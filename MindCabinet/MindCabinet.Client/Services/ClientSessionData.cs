using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserContext;
using MindCabinet.Shared.Utility;
using System.Net.Http.Json;
using System.Text.Json;

namespace MindCabinet.Client.Services;


public partial class ClientSessionData( INetMode netMode, IServiceScopeFactory serviceScopeFactory ) {
    public class SessionDataJson {
        public string SessionId { get; set; } = "";
        public SimpleUserObject.ClientData? UserData { get; set; }
        public UserAppDataObject? UserAppData { get; set; }
    }



    private readonly INetMode NetMode = netMode;

    private readonly IServiceScopeFactory ServiceScopeFactory = serviceScopeFactory;

    public bool IsLoaded { get; private set; } = false;

    public bool IsLoading { get; private set; } = false;


    private SessionDataJson? ServerData;



    public const string Get_Path = "Session";
    public const string Get_Route = "GetSession";

    internal async Task<bool> Load_Async() {
        if( !this.NetMode.IsClientSide ) {
            return false;
        }

        if( this.IsLoaded || this.IsLoading ) {
            return false;
        }
        this.IsLoading = true;
        
        using IServiceScope scope = this.ServiceScopeFactory.CreateScope();
        HttpClient? httpClient = scope.ServiceProvider.GetService<HttpClient>();
        if( httpClient is null ) {
            throw new InvalidOperationException( "HttpClient service not available in ClientSessionData." );
        }

        //ClientSessionData.Json? data = await this.Http.GetFromJsonAsync<ClientSessionData.Json>( "Session/Data" );
        HttpResponseMessage msg = await httpClient.PostAsJsonAsync(
            $"{Get_Path}/{Get_Route}",
            new object()
        );

        msg.EnsureSuccessStatusCode();

        string rawData = await msg.Content.ReadAsStringAsync();
        if( string.IsNullOrWhiteSpace(rawData) || rawData == "{}" ) {
            throw new InvalidDataException( "Could not deserialize raw ClientSessionData.SessionDataJson" );
        }

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        ClientSessionData.SessionDataJson? data = JsonSerializer.Deserialize<ClientSessionData.SessionDataJson>(
            json: rawData,
            options: options
        );
        // ClientSessionData.SessionDataJson? data = await msg.Content
        //     .ReadFromJsonAsync<ClientSessionData.SessionDataJson>();
        if( data is null ) {
            throw new InvalidDataException( "Could not deserialize ClientSessionData.SessionDataJson" );
        }

        this.ServerData = data;

        this.IsLoading = false;
        this.IsLoaded = true;

        return true;
    }
}
