using MindCabinet.Shared.Utility;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace MindCabinet.Shared.DataEntries;


public class SimpleUserEntry : IEquatable<SimpleUserEntry> {
	public static (bool, string) ValidateUserName( string name ) {
		if( string.IsNullOrEmpty(name) ) {
			return (false, "Empty");
		}
		if( name.Length < 3 ) {
			return (false, "Too short");
		}
		if( name.Any(Char.IsWhiteSpace) ) {
            return (false, "Contains whitespace(s)");
        }
		if( !name.All(Char.IsLetterOrDigit) ) {
            return (false, "Contains non-letter or non-digit characters");
        }
		//if( Char.IsDigit(name[0]) ) {
		//    return (false, "Starts with digit");
		//}
		return (true, "Is valid");
	}

	public static (bool, string) ValidateEmailAddress( string name ) {
		if( string.IsNullOrEmpty(name) ) {
			return (false, "Empty");
		}

        var email = new EmailAddressAttribute();
        if( !email.IsValid(name) ) {
            return (false, "Invalid email address");
        }
		return (true, "Is valid");
	}

	public static string GeneratePwSalt() {
		return Misc.GetRandomString( 32 );
	}

	//



	public long? Id { get; private set; } = null;

	[JsonIgnore]
	public bool IsAssignedId { get; private set; } = false;


	public DateTime Created { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }

    //public string Hash { get; set; }

    //public string Salt { get; set; }

    public bool IsValidated { get; set; }



    public SimpleUserEntry() {
		this.Name = string.Empty;
		this.Email = string.Empty;
	}

	public SimpleUserEntry(
				DateTime created,
				string name,
				string email,
				//string hash,
				//string salt,
				bool isValidated ) {
		this.Created = created;
		this.Name = name;
		this.Email = email;
        //this.Hash = hash;
        //this.Salt = salt;
        this.IsValidated = isValidated;
	}

	public SimpleUserEntry(
				long id,
				DateTime created,
				string name,
				string email,
				//string hash,
				//string salt,
				bool isValidated ) {
        this.Id = id;
        this.Created = created;
        this.Name = name;
        this.Email = email;
        //this.Hash = hash;
        //this.Salt = salt;
        this.IsValidated = isValidated;
    }


	public bool Equals( SimpleUserEntry? other ) {
		if( other is null ) { return false; }
		if( this == other ) { return true; }

		if( this.Id != other.Id ) { return false; }
		if( this.ContentEquals(other, true) ) { return false; }
		return true;
	}

	public bool ContentEquals( SimpleUserEntry other, bool includeCreateDate ) {
        if( includeCreateDate && this.Created != other.Created ) { return false; }
        if( this.Name != other.Name ) { return false; }
		if( this.Email != other.Email ) { return false; }
		//if( this.Hash != other.Hash ) { return false; }
		//if( this.Salt != other.Salt ) { return false; }
		if( this.IsValidated != other.IsValidated ) { return false; }
		return true;
	}
}
