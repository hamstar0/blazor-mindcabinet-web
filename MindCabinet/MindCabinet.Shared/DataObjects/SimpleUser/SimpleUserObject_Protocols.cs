using MindCabinet.Shared.Utility;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using static MindCabinet.Shared.DataObjects.SimpleUserObject;


namespace MindCabinet.Shared.DataObjects;


public partial class SimpleUserObject {
    public static Raw CreateRaw(
            SimpleUserId id,
            DateTime created,
            string name,
            string email,
            byte[] pwHash,
            byte[] pwSalt,
            bool isValidated
            //bool isPrivileged
        ) {
        return new Raw {
            Id = id,
            Created = created,
            Name = name,
            Email = email,
            PwHash = pwHash,
            PwSalt = pwSalt,
            IsValidated = isValidated
            //IsPrivileged = isPrivileged
        };
    }

    public class Raw : IRawDataObject, IHasId<SimpleUserId> {
        public SimpleUserId Id { get; set; }
        public DateTime Created { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public byte[] PwHash { get; set; } = new byte[ SimpleUserObject.PasswordHashLength ];
        public byte[] PwSalt { get; set; } = new byte[ SimpleUserObject.PasswordSaltLength ];
        public bool IsValidated { get; set; } = false;
        //public bool IsPrivileged { get; set; } = false;


        public SimpleUserObject CreateDataObject() {
            return new SimpleUserObject(
                id: this.Id,
                created: this.Created,
                name: this.Name,
                email: this.Email,
                pwHash: this.PwHash,
                pwSalt: this.PwSalt,
                isValidated: this.IsValidated
                //isPrivileged: this.IsPrivileged
            );
        }
    }


    public static UserAndSession_Raw CreateUserAndSessionRaw(
            SimpleUserId simpleUserId,
            DateTime created,
            string name,
            string email,
            string sessionId,
            string latestIpAddress,
            byte[] pwHash,
            byte[] pwSalt,
            bool isValidated,
            //bool isPrivileged,
            DateTime firstVisit,
            DateTime latestVisit,
            int visits ) {
        return new UserAndSession_Raw {
            Id = simpleUserId,
            Created = created,
            Name = name,
            Email = email,
            SessionId = sessionId,
            LatestIpAddress = latestIpAddress,
            PwHash = pwHash,
            PwSalt = pwSalt,
            IsValidated = isValidated,
            //IsPrivileged = isPrivileged,
            FirstVisit = firstVisit,
            LatestVisit = latestVisit,
            Visits = visits
        };
    }
    
    public class UserAndSession_Raw : IRawDataObject, IHasId<SimpleUserId> {
        public SimpleUserId Id { get; set; }
        public DateTime Created { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public byte[] PwHash { get; set; } = new byte[SimpleUserObject.PasswordHashLength];
        public byte[] PwSalt { get; set; } = new byte[SimpleUserObject.PasswordSaltLength];
        public bool IsValidated { get; set; } = false;
        //public bool IsPrivileged { get; set; } = false;

        public string SessionId { get; set; } = "";   // Note: MySessions.Id is SessionId to avoid collision
        public string LatestIpAddress { get; set; } = "";
        public SimpleUserId SimpleUserId { get; set; }
        public DateTime FirstVisit { get; set; }
        public DateTime LatestVisit { get; set; }
        public int Visits { get; set; }


        public UserAndSession_Raw() {}

        public Raw GetUserRaw() {
            return SimpleUserObject.CreateRaw(
                id: this.Id,
                created: this.Created,
                name: this.Name,
                email: this.Email,
                pwHash: this.PwHash,
                pwSalt: this.PwSalt,
                isValidated: this.IsValidated
                //isPrivileged: this.IsPrivileged
            );
        }
    }


    
    public class ClientObject( SimpleUserId id, string name, DateTime created, string email ) : IRawDataObject, IHasId<SimpleUserId> {
        public SimpleUserId Id { get; set; } = id;
        public string Name { get; set; } = name;
        public DateTime Created { get; set; } = created;
        public string Email { get; set; } = email;
    }
}
