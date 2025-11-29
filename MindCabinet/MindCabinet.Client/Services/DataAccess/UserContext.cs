using System.Net.Http.Json;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.UserContext;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_UserContext : IClientDataAccess {
    private HttpClient Http;


    internal ClientDataAccess_UserContext( HttpClient http ) {
        this.Http = http;
    }


    public class GetByUserId_Params( long userId ) {
        public long UserId { get; } = userId;
    }

    public const string GetByUserId_Path = "Context";
    public const string GetByUserId_Route = "GetByUserId";

    public async Task<IEnumerable<UserContext>> GetByUserId_Async( GetByUserId_Params parameters ) {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            $"{GetByUserId_Path}/{GetByUserId_Route}",
            parameters
        );
        
        msg.EnsureSuccessStatusCode();
        
        IEnumerable<UserContext>? ret = await msg.Content.ReadFromJsonAsync<IEnumerable<UserContext>>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize IEnumerable<UserContext>" );
        }

        return ret;
    }


    public class Create_Params(
                string name,
                List<UserContextEntry> entries ) {
        public string Name { get; } = name;
        public List<UserContextEntry> Context { get; } = entries;
    }

    public class Create_Return(
                bool isAdded,
                UserContext context ) {
        public bool IsAdded { get; } = isAdded;
        public UserContext Context { get; } = context;
    }

    public const string Create_Path = "UserContext";
    public const string Create_Route = "Create";
    
    public async Task<Create_Return> Create_Async( Create_Params parameters ) {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            $"{Create_Path}/{Create_Route}",
            parameters
        );

        msg.EnsureSuccessStatusCode();

        Create_Return? ret = await msg.Content.ReadFromJsonAsync<Create_Return>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize UserContext" );
        }

        return ret;
    }
}
