using System.Net.Http.Json;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects;


namespace MindCabinet.Client.Services.DbAccess;


public class ClientDataAccess_SimpleUsers( HttpClient http ) : IClientDataAccess {
    private readonly HttpClient Http = http;


    //public class GetSimpleUsersByCriteriaParams(
    //            string? namePattern,
    //            string? emailPattern,
    //            DateTime? createdBefore,
    //            DateTime? createdAfter ) {
    //    public string? NamePattern { get; } = namePattern;
    //    public string? EmailPattern { get; } = emailPattern;
    //    public DateTime? CreatedBefore { get; } = createdBefore;
    //    public DateTime? CreatedAfter { get; } = createdAfter;
    //}

    public class Create_Params(
                string name,
                string email,
                string password,
                bool isValidated ) {
        public string Name { get; } = name;
        public string Email { get; } = email;
        public string Password { get; } = password;
        public bool IsValidated { get; } = isValidated;
    }

    public const string Create_Path = "SimpleUser";
    public const string Create_Route = "Create";

    public async Task<SimpleUserObject.ClientData> Create_Async( Create_Params parameters ) {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            requestUri: $"{Create_Path}/{Create_Route}",
            value: parameters
        );

        msg.EnsureSuccessStatusCode();

        SimpleUserObject.ClientData? ret = await msg.Content.ReadFromJsonAsync<SimpleUserObject.ClientData>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize SimpleUserEntry" );
        }

        return ret;
    }


    public class Login_Params(
                string name,
                string password) {
        public string Name { get; } = name;
        public string Password { get; } = password;
    }

    public class Login_Return( SimpleUserObject.ClientData? user, string status ) {
        public SimpleUserObject.ClientData? User { get; } = user;
        public string Status { get; } = status;
    }

    public const string Login_Path = "SimpleUser";
    public const string Login_Route = "Login";

    public async Task<Login_Return> Login_Async( Login_Params parameters ) {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            requestUri: $"{Login_Path}/{Login_Route}",
            value: parameters
        );

        msg.EnsureSuccessStatusCode();

        Login_Return? ret = await msg.Content.ReadFromJsonAsync<Login_Return>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize SimpleUserLoginReply" );
        }

        return ret;
    }
}
