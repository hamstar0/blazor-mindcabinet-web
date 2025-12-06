using System.Net.Http.Json;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.UserContext;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_UserContext( HttpClient http ) : IClientDataAccess {
    private HttpClient Http = http;


    public class Get_Return( IEnumerable<UserContextObject.UserContextWithTermEntries_DbData> contexts ) {
        public IEnumerable<UserContextObject.UserContextWithTermEntries_DbData> Contexts { get; } = contexts;
    }

    public const string GetForCurrentUser_Path = "UserContext";
    public const string GetForCurrentUser_Route = "GetForCurrentUser";

    public async Task<IEnumerable<UserContextObject>> GetForCurrentUser_Async() {
        HttpResponseMessage msg = await this.Http.GetAsync(
            $"{GetForCurrentUser_Path}/{GetForCurrentUser_Route}"
        );
        
        msg.EnsureSuccessStatusCode();
        
        IEnumerable<UserContextObject>? ret = await msg.Content.ReadFromJsonAsync<IEnumerable<UserContextObject>>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize IEnumerable<UserContext>" );
        }

        return ret;
    }


    // public class GetByUserId_Params( long userId ) {
    //     public long UserId { get; } = userId;
    // }

    // public class GetByUserId_Return( IEnumerable<UserContext.UserContextWithTermEntries_DbData> contexts ) {
    //     public IEnumerable<UserContext.UserContextWithTermEntries_DbData> Contexts { get; } = contexts;
    // }

    // public const string GetByUserId_Path = "UserContext";
    // public const string GetByUserId_Route = "GetByUserId";

    // public async Task<IEnumerable<UserContext>> GetByUserId_Async( GetByUserId_Params parameters ) {
    //     HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
    //         $"{GetByUserId_Path}/{GetByUserId_Route}",
    //         parameters
    //     );
        
    //     msg.EnsureSuccessStatusCode();
        
    //     IEnumerable<UserContext>? ret = await msg.Content.ReadFromJsonAsync<IEnumerable<UserContext>>();
    //     if( ret is null ) {
    //         throw new InvalidDataException( "Could not deserialize IEnumerable<UserContext>" );
    //     }

    //     return ret;
    // }


    public class CreateForCurrentUser_Params(
                string name,
                List<UserContextEntryObject> entries ) {
        public string Name { get; } = name;
        public List<UserContextEntryObject> Entries { get; } = entries;
    }

    public class CreateForCurrentUser_Return(
                long userContextId ) {
        public long UserContextId { get; } = userContextId;
    }

    public const string CreateForCurrentUser_Path = "UserContext";
    public const string CreateForCurrentUser_Route = "CreateForCurrentUser";
    
    public async Task<CreateForCurrentUser_Return> CreateForCurrentUser_Async( CreateForCurrentUser_Params parameters ) {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            $"{CreateForCurrentUser_Path}/{CreateForCurrentUser_Route}",
            parameters
        );

        msg.EnsureSuccessStatusCode();

        CreateForCurrentUser_Return? ret = await msg.Content.ReadFromJsonAsync<CreateForCurrentUser_Return>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize UserContext" );
        }

        return ret;
    }
}
