using MindCabinet.Shared.Utility;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using static MindCabinet.Shared.DataObjects.SimpleUserObject;


namespace MindCabinet.Shared.DataObjects;


public partial class SimpleUserObject : IEquatable<SimpleUserObject> {
    public class User_Raw {
        public long Id;
        public DateTime Created;
        public string Name = "";
        public string Email = "";
        public byte[] PwHash = new byte[SimpleUserObject.PasswordHashLength];
        public byte[] PwSalt = new byte[SimpleUserObject.PasswordSaltLength];
        public bool IsValidated = false;
        //public bool IsPrivileged = false;
        
        
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

    public class Session_Raw {
        public string Id = "";
        public string IpAddress = "";
        public long SimpleUserId;
        public DateTime FirstVisit;
        public DateTime LatestVisit;
        public int Visits;
    }

    public class UserAndSession_Raw : User_Raw {    // no multiple inheritance :(
        public string SessionId = "";
        public string IpAddress = "";
        public long SimpleUserId;
        public DateTime FirstVisit;
        public DateTime LatestVisit;
        public int Visits;
    }


    
    public class ClientObject( long id, string name, DateTime created, string email ) {
        public long Id { get; } = id;
        public string Name { get; } = name;
        public DateTime Created { get; } = created;
        public string Email { get; } = email;
    }
}
