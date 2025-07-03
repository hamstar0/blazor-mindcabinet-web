using System.Net.Http.Json;
using MindCabinet.Shared.DataEntries;


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


    public async Task<SimpleUserEntry> CreateSimpleUser_Async( CreateSimpleUserParams parameters ) {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync( "SimpleUser/Create", parameters );

        msg.EnsureSuccessStatusCode();

        SimpleUserEntry? ret = await msg.Content.ReadFromJsonAsync<SimpleUserEntry>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize SimpleUserEntry" );
        }

        return ret;
    }
}
