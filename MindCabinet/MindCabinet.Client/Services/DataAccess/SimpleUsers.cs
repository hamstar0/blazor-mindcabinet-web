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

    public class Create_Params {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public bool IsValidated { get; set; }
    }
    
    public class Create_Return {
        public SimpleUserObject.ClientObject? User { get; set; }
        public string Status { get; set; } = "";
    }

    public const string Create_Path = "SimpleUser";
    public const string Create_Route = "Create";

    public async Task<Create_Return> Create_Async( Create_Params parameters ) {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            requestUri: $"{Create_Path}/{Create_Route}",
            value: parameters
        );

        msg.EnsureSuccessStatusCode();

        Create_Return? ret = await msg.Content.ReadFromJsonAsync<Create_Return>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize Create_Return" );
        }

        return ret;
    }


    public class Login_Params {
        public string Name { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class Login_Return {
        public SimpleUserObject.ClientObject? User { get; set; }
        public string Status { get; set; } = "";
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
