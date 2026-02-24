using MindCabinet.Shared.Utility;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;


namespace MindCabinet.Shared.DataObjects;


public partial class SimpleUserObject : IEquatable<SimpleUserObject> {
	public long Id { get; private set; }

	public DateTime Created { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }

	public byte[] PwHash { get; set; }

	public byte[] PwSalt { get; set; }

	public bool IsValidated { get; set; }



	public SimpleUserObject(
				long id,
				DateTime created,
				string name,
				string email,
				byte[] pwHash,
				byte[] pwSalt,
				bool isValidated ) {
        this.Id = id;
        this.Created = created;
        this.Name = name;
        this.Email = email;
		this.PwHash = pwHash;
		this.PwSalt = pwSalt;
		this.IsValidated = isValidated;
    }


	public bool Equals( SimpleUserObject? other ) {
		if( other is null ) { return false; }
		if( this == other ) { return true; }

		if( this.Id != other.Id ) { return false; }
		if( this.ContentEquals(other, true, true, true) ) { return false; }
		return true;
	}

	public bool ContentEquals(
				SimpleUserObject other,
				bool includeCreateDate,
				bool includePw,
				bool includeValidation ) {
        if( includeCreateDate && this.Created != other.Created ) { return false; }
        if( this.Name != other.Name ) { return false; }
		if( this.Email != other.Email ) { return false; }
		if( includePw && !CryptographicOperations.FixedTimeEquals(this.PwHash, other.PwHash) ) { return false; }
		if( includePw && this.PwSalt != other.PwSalt ) { return false; }
		if( includeValidation && this.IsValidated != other.IsValidated ) { return false; }
		return true;
	}
}
