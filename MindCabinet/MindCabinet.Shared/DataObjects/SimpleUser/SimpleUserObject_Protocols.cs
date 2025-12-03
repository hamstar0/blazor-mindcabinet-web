using MindCabinet.Shared.Utility;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using static MindCabinet.Shared.DataObjects.SimpleUserObject;


namespace MindCabinet.Shared.DataObjects;


public partial class SimpleUserObject : IEquatable<SimpleUserObject> {
    public class SimpleUser_DbData {
        public SimpleUserObject CreateUserEntry() {
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
        
        public long Id;
        public DateTime Created;
        public string Name = "";
        public string Email = "";
        public byte[] PwHash = new byte[SimpleUserObject.PasswordHashLength];
        public byte[] PwSalt = new byte[SimpleUserObject.PasswordSaltLength];
        public bool IsValidated = false;
        //public bool IsPrivileged = false;
    }

    public class Session_DbData {
        public string Id = "";
        public string IpAddress = "";
        public long SimpleUserId;
        public DateTime FirstVisit;
        public DateTime LatestVisit;
        public int Visits;
    }

    public class SimpleUserAndSession_DbData : SimpleUser_DbData {    // no multiple inheritance
        public string SessionId = "";
        public string IpAddress = "";
        public long SimpleUserId;
        public DateTime FirstVisit;
        public DateTime LatestVisit;
        public int Visits;
    }


    
    public class ClientData( long id, string name, DateTime created, string email ) {
        public long Id { get; } = id;
        public string Name { get; } = name;
        public DateTime Created { get; } = created;
        public string Email { get; } = email;
    }

    public ClientData GetClientOnlyData() {
		return new ClientData( this.Id, this.Name, this.Created, this.Email );
	}
}
