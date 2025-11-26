using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using System.Net.Http.Json;

namespace MindCabinet.Client.Services;


public partial class ClientSessionData {
    public class RawServerData(
                string sessionId,
                SimpleUserObject.ClientData? userData,
                List<TermObject> favoriteTerms,
                List<TermObject> recentTerms ) {
        public string SessionId = sessionId;
        public SimpleUserObject.ClientData? UserData = userData;

        public List<TermObject> FavoriteTerms = favoriteTerms;

        public List<TermObject> RecentTerms = recentTerms;
    }



    private HttpClient Http;
    

    public bool IsLoaded { get; private set; } = false;


    private RawServerData? ServerData;

    public string? SessionId { get => this.ServerData?.SessionId; }

    public string? UserName { get => this.ServerData?.UserData?.Name; }


    private List<TermObject> CurrentContextTags = new List<TermObject>();



    public ClientSessionData( HttpClient http ) {
        this.Http = http;
    }



    public const string Session_GetSessionData_Path = "Session";
    public const string Session_GetSessionData_Route = "GetSessionData";

    internal async Task Load_Async() {
        //ClientSessionData.Json? data = await this.Http.GetFromJsonAsync<ClientSessionData.Json>( "Session/Data" );
        HttpResponseMessage msg = await this.Http.GetAsync(
            $"{Session_GetSessionData_Path}/{Session_GetSessionData_Route}"
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
