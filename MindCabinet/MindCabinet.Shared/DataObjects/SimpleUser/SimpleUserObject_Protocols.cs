using MindCabinet.Shared.Utility;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using static MindCabinet.Shared.DataObjects.SimpleUserObject;


namespace MindCabinet.Shared.DataObjects;


public partial class SimpleUserObject : IEquatable<SimpleUserObject> {
    public class User_Raw : IRawDataObject {
        public long Id { get; set; }
        public DateTime Created { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public byte[] PwHash { get; set; } = new byte[SimpleUserObject.PasswordHashLength];
        public byte[] PwSalt { get; set; } = new byte[SimpleUserObject.PasswordSaltLength];
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

    public class Session_Raw : IRawDataObject {
        public string Id { get; set; } = "";
        public string LatestIpAddress { get; set; } = "";
        public long SimpleUserId { get; set; }
        public DateTime FirstVisit { get; set; }
        public DateTime LatestVisit { get; set; }
        public int Visits { get; set; }
    }

    public class UserAndSession_Raw : User_Raw, IRawDataObject {
        public string SessionId { get; set; } = "";   // Note: MySessions.Id is SessionId to avoid collision
        public string LatestIpAddress { get; set; } = "";
        public long SimpleUserId { get; set; }
        public DateTime FirstVisit { get; set; }
        public DateTime LatestVisit { get; set; }
        public int Visits { get; set; }
    }


    
    public class ClientObject( long id, string name, DateTime created, string email ) : IRawDataObject {
        public long Id { get; set; } = id;
        public string Name { get; set; } = name;
        public DateTime Created { get; set; } = created;
        public string Email { get; set; } = email;
    }
}
