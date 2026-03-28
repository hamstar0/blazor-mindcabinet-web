using System.Net.Http.Json;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.UserPostsContext;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_UserPostsContext(
            HttpClient http,
            ClientSessionData sessionData
        ) : IClientDataAccess {
    private HttpClient Http = http;

    private ClientSessionData SessionData = sessionData;



    public class GetForCurrentUserByCriteria_Params {
        public string? NameContains { get; set; }

        public UserPostsContextId[] Ids { get; set; } = [];
    }

    public class Get_Return( IEnumerable<UserPostsContextObject.Raw> contexts ) {
        public IEnumerable<UserPostsContextObject.Raw> Contexts { get; } = contexts;
    }

    public const string GetForCurrentUserByCriteria_Path = "UserPostsContext";
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

    // public class GetByUserId_Return( IEnumerable<UserPostsContext.UserPostsContextWithTermEntries_DbData> contexts ) {
    //     public IEnumerable<UserPostsContext.UserPostsContextWithTermEntries_DbData> Contexts { get; } = contexts;
    // }

    // public const string GetByUserId_Path = "UserPostsContext";
    // public const string GetByUserId_Route = "GetByUserId";

    // public async Task<IEnumerable<UserPostsContextObject>> GetByUserId_Async( GetByUserId_Params parameters ) {
    //     HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
    //         $"{GetByUserId_Path}/{GetByUserId_Route}",
    //         parameters
    //     );
        
    //     msg.EnsureSuccessStatusCode();
        
    //     IEnumerable<UserPostsContextObject>? ret = await msg.Content.ReadFromJsonAsync<IEnumerable<UserPostsContextObject>>();
    //     if( ret is null ) {
    //         throw new InvalidDataException( "Could not deserialize IEnumerable<UserPostsContextObject>" );
    //     }

    //     return ret;
    // }

    public class CreateOrUpdate_Return( UserPostsContextId id ) {
        public UserPostsContextId Id { get; } = id;
    }

    public const string CreateForCurrentUser_Path = "UserPostsContext";
    public const string CreateForCurrentUser_Route = "CreateForCurrentUser";
    
    public async Task<CreateOrUpdate_Return> CreateForCurrentUser_Async( UserPostsContextObject.Raw parameters ) {
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
    

    public const string UpdateForCurrentUser_Path = "UserPostsContext";
    public const string UpdateForCurrentUser_Route = "UpdateForCurrentUser";
    
    public async Task<CreateOrUpdate_Return> UpdateForCurrentUser_Async( UserPostsContextObject.Prototype parameters ) {
        if( this.SessionData.UserId is null ) {
            throw new InvalidOperationException( "No user in session" );
        }
        if( parameters.Id is null || parameters.Id == 0 ) {
            throw new ArgumentException( "UserPostsContextObject.Prototype Id is not valid (must be non-zero and non-null)." );
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
