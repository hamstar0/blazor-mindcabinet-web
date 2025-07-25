﻿using MindCabinet.Shared.Utility;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using static MindCabinet.Shared.DataObjects.SimpleUserObject;


namespace MindCabinet.Shared.DataObjects;


public static class SimpleUserEntryExt {
    public static SimpleUserObject CreateUserEntry( this UserDbData data ) {
        return new SimpleUserObject(
            id: data.Id,
            created: data.Created,
            name: data.Name,
            email: data.Email,
            pwHash: data.PwHash,
            pwSalt: data.PwSalt,
            isValidated: data.IsValidated
        //isPrivileged: data.IsPrivileged
        );
    }
}



public partial class SimpleUserObject : IEquatable<SimpleUserObject> {
    public class ClientData( long id, string name, DateTime created, string email ) {
        public long Id { get; } = id;
        public string Name { get; } = name;
        public DateTime Created { get; } = created;
        public string Email { get; } = email;
    }


    public class UserDbData {
        public long Id;
        public DateTime Created;
        public string Name = "";
        public string Email = "";
        public byte[] PwHash = new byte[SimpleUserObject.PasswordHashLength];
        public byte[] PwSalt = new byte[SimpleUserObject.PasswordSaltLength];
        public bool IsValidated = false;
        //public bool IsPrivileged = false;
    }


    public class SessionDbData {
        public string Id = "";
        public string IpAddress = "";
        public long SimpleUserId;
        public DateTime FirstVisit;
        public DateTime LatestVisit;
        public int Visits;
    }

    public class UserAndSessionDbData : UserDbData {    // multiple inheritance?
        public string SessionId = "";
        public string IpAddress = "";
        public long SimpleUserId;
        public DateTime FirstVisit;
        public DateTime LatestVisit;
        public int Visits;
    }



    public ClientData GetClientOnlyData() {
		return new ClientData( this.Id ?? 0, this.Name, this.Created, this.Email );
	}
}
