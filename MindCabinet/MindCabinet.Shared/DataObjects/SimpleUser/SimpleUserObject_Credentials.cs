using MindCabinet.Shared.Utility;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;


namespace MindCabinet.Shared.DataObjects;


public partial class SimpleUserObject : IEquatable<SimpleUserObject> {
	public const int PasswordHashLength = 32;
	public const int PasswordSaltLength = 16;



	public static string GeneratePwSalt() {
		return Misc.GetRandomString( 32 );
	}

    //public static byte[] GetPasswordHash( string password, byte[] pwSalt ) {
    //    var argon2id = new Argon2id( Encoding.UTF8.GetBytes( password ) );
    //    argon2id.Salt = pwSalt;
    //    argon2id.MemorySize = 12288;
    //    argon2id.Iterations = 2;
    //    argon2id.DegreeOfParallelism = 1;

    //    return argon2id.GetBytes( SimpleUserEntry.PasswordHashLength );
    //}


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
}
