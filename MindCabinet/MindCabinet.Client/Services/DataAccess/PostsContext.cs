using System.Net.Http.Json;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.PostsContext;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_PostsContext(
            HttpClient http,
            ClientSessionManager sessionData
        ) : IClientDataAccess {
    private HttpClient Http = http;

    private ClientSessionManager SessionData = sessionData;



    public class GetForCurrentUserByCriteria_Params {
        public string? NameContains { get; set; }

        public PostsContextId[] Ids { get; set; } = [];
    }

    public class Get_Return {
        public IEnumerable<PostsContextObject.Raw> Contexts { get; set; } = [];
    }

    public const string GetForCurrentUserByCriteria_Path = "PostsContext";
    public const string GetForCurrentUserByCriteria_Route = "GetForCurrentUserByCriteria";

    public async Task<Get_Return> GetForCurrentUserByCriteria_Async(
                GetForCurrentUserByCriteria_Params parameters ) {
        if( this.SessionData.UserId is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            requestUri: $"{GetForCurrentUserByCriteria_Path}/{GetForCurrentUserByCriteria_Route}",
            value: parameters
        );
        
        msg.EnsureSuccessStatusCode();
        
        Get_Return? ret = await msg.Content.ReadFromJsonAsync<Get_Return>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize Get_Return" );
        }

        return ret;
    }


    // public class GetByUserId_Params( SimpleUserId userId ) {
    //     public SimpleUserId UserId { get; } = userId;
    // }

    // public class GetByUserId_Return( IEnumerable<PostsContext.PostsContextWithTermEntries_DbData> contexts ) {
    //     public IEnumerable<PostsContext.PostsContextWithTermEntries_DbData> Contexts { get; } = contexts;
    // }

    // public const string GetByUserId_Path = "PostsContext";
    // public const string GetByUserId_Route = "GetByUserId";

    // public async Task<IEnumerable<PostsContextObject>> GetByUserId_Async( GetByUserId_Params parameters ) {
    //     HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
    //         $"{GetByUserId_Path}/{GetByUserId_Route}",
    //         parameters
    //     );
        
    //     msg.EnsureSuccessStatusCode();
        
    //     IEnumerable<PostsContextObject>? ret = await msg.Content.ReadFromJsonAsync<IEnumerable<PostsContextObject>>();
    //     if( ret is null ) {
    //         throw new InvalidDataException( "Could not deserialize IEnumerable<PostsContextObject>" );
    //     }

    //     return ret;
    // }

    public class CreateOrUpdate_Return {
        public PostsContextId Id { get; set; }
    }

    public const string CreateForCurrentUser_Path = "PostsContext";
    public const string CreateForCurrentUser_Route = "CreateForCurrentUser";
    
    public async Task<CreateOrUpdate_Return> CreateForCurrentUser_Async( PostsContextObject.Raw parameters ) {
        if( this.SessionData.UserId is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            requestUri: $"{CreateForCurrentUser_Path}/{CreateForCurrentUser_Route}",
            value: parameters
        );

        msg.EnsureSuccessStatusCode();

        CreateOrUpdate_Return? ret = await msg.Content.ReadFromJsonAsync<CreateOrUpdate_Return>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize CreateForCurrentUser_Return" );
        }

        return ret;
    }
    

    public const string UpdateForCurrentUser_Path = "PostsContext";
    public const string UpdateForCurrentUser_Route = "UpdateForCurrentUser";
    
    public async Task<CreateOrUpdate_Return> UpdateForCurrentUser_Async( PostsContextObject.Prototype parameters ) {
        if( this.SessionData.UserId is null ) {
            throw new InvalidOperationException( "No user in session" );
        }
        if( parameters.Id is null || parameters.Id == 0 ) {
            throw new ArgumentException( "PostsContextObject.Prototype Id is not valid (must be non-zero and non-null)." );
        }

        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            requestUri: $"{UpdateForCurrentUser_Path}/{UpdateForCurrentUser_Route}",
            value: parameters
        );

        msg.EnsureSuccessStatusCode();

        CreateOrUpdate_Return? ret = await msg.Content.ReadFromJsonAsync<CreateOrUpdate_Return>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize CreateForCurrentUser_Return (Update)" );
        }

        return ret;
    }
}
