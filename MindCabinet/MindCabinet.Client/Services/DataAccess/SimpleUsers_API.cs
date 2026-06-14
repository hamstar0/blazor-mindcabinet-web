using System.Net.Http.Json;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects;


namespace MindCabinet.Client.Services.DbAccess;


public partial class ClientDataAccess_SimpleUsers : IClientDataAccess {
    public interface IAPI : IServerDataAccessAPI {
        public const string BaseRoute = "SimpleUsers";



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

        public Task<Create_Return> Create_Async( Create_Params parameters );



        public class Login_Params {
            public string Name { get; set; } = "";
            public string Password { get; set; } = "";
        }

        public class Login_Return {
            public SimpleUserObject.ClientObject? User { get; set; }
            public string Status { get; set; } = "";
        }

        public Task<Login_Return> Login_Async( Login_Params parameters );
    }
}
