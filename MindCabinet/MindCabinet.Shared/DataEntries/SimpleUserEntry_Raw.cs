using MindCabinet.Shared.Utility;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace MindCabinet.Shared.DataEntries;


public partial class SimpleUserEntry : IEquatable<SimpleUserEntry> {
    public class DbData {
        public long Id;
        public DateTime Created;
        public string Name = "";
        public string Email = "";
        public byte[] PwHash = new byte[SimpleUserEntry.PasswordHashLength];
        public byte[] PwSalt = new byte[SimpleUserEntry.PasswordSaltLength];
        public bool IsValidated = false;
        //public bool IsPrivileged = false;
        

        public SimpleUserEntry CreateUserEntry() {
            return new SimpleUserEntry(
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

    public class ClientData( long id, string name, DateTime created, string email ) {
        public long Id { get; } = id;
        public string Name { get; } = name;
        public DateTime Created { get; } = created;
        public string Email { get; } = email;
    }


    public class SessionData {
        public string SessionId;
        public string Ip;
        public long SimpleUserId;
        public DateTime FirstVisit;
        public DateTime LatestVisit;
        public int Visits;

        public SessionData() {
            this.SessionId = "";
            this.Ip = "";
        }

        public SessionData(
                    string sessionId,
                    string ip,
                    long simpleUserId,
                    DateTime firstVisit,
                    DateTime latestVisit,
                    int visits ) {
            this.SessionId = sessionId;
            this.Ip = ip;
            this.SimpleUserId = simpleUserId;
            this.FirstVisit = firstVisit;
            this.LatestVisit = latestVisit;
            this.Visits = visits;
        }
    }



    public ClientData GetClientOnlyData() {
		return new ClientData( this.Id ?? 0, this.Name, this.Created, this.Email );
	}
}
