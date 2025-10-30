using System.Net.Http.Json;
using MindCabinet.Shared.DataObjects;


namespace MindCabinet.Client.Services;



public partial class ClientDbAccess {
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

    public class CreateSimpleUserParams(
                string name,
                string email,
                string password,
                bool isValidated ) {
        public string Name { get; } = name;
        public string Email { get; } = email;
        public string Password { get; } = password;
        public bool IsValidated { get; } = isValidated;
    }

    public readonly static (string path, string route) Route_SimpleUser_Create = ("SimpleUser", "Create");

    public async Task<SimpleUserObject.ClientData> CreateSimpleUser_Async( CreateSimpleUserParams parameters ) {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            ClientDbAccess.Route_SimpleUser_Create.path + "/" + ClientDbAccess.Route_SimpleUser_Create.route,
            parameters
        );

        msg.EnsureSuccessStatusCode();

        SimpleUserObject.ClientData? ret = await msg.Content.ReadFromJsonAsync<SimpleUserObject.ClientData>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize SimpleUserEntry" );
        }

        return ret;
    }


    public class LoginSimpleUserParams(
                string name,
                string password) {
        public string Name { get; } = name;
        public string Password { get; } = password;
    }

    public class SimpleUserLoginReply( SimpleUserObject.ClientData? user, string status ) {
        public SimpleUserObject.ClientData? User { get; } = user;
        public string Status { get; } = status;
    }

    public readonly static (string path, string route) Route_SimpleUser_Login = ("SimpleUser", "Login");

    public async Task<SimpleUserLoginReply> LoginSimpleUser_Async( LoginSimpleUserParams parameters ) {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            ClientDbAccess.Route_SimpleUser_Login.path + "/" + ClientDbAccess.Route_SimpleUser_Login.route,
            parameters
        );

        msg.EnsureSuccessStatusCode();

        SimpleUserLoginReply? ret = await msg.Content.ReadFromJsonAsync<SimpleUserLoginReply>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize SimpleUserLoginReply" );
        }

        return ret;
    }
}
