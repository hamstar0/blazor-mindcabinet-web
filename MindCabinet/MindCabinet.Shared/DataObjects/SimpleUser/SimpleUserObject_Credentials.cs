using MindCabinet.Shared.Utility;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;


namespace MindCabinet.Shared.DataObjects;


public partial class SimpleUserObject : IEquatable<SimpleUserObject> {
	public const int PasswordHashLength = 32;
	public const int PasswordSaltLength = 32;	//16;



	public static string GeneratePwSalt() {
		return Misc.GetRandomString( SimpleUserObject.PasswordSaltLength );
	}

    //public static byte[] GetPasswordHash( string password, byte[] pwSalt ) {
    //    var argon2id = new Argon2id( Encoding.UTF8.GetBytes( password ) );
    //    argon2id.Salt = pwSalt;
    //    argon2id.MemorySize = 12288;
    //    argon2id.Iterations = 2;
    //    argon2id.DegreeOfParallelism = 1;

    //    return argon2id.GetBytes( SimpleUserEntry.PasswordHashLength );
    //}


	
    public enum StatusCode {
        OK = 0,
        NAME_EMPTY = 1,
        NAME_SHORT = 2,
        NAME_LONG = 4,
        NAME_WS_PADDED = 8,
        NAME_WS_CONSEC = 16,
        NAME_WS_MISC = 32,
        NAME_UNICODE = 64,
        EMAIL_EMPTY = 128,
        EMAIL_MALFORMED = 256,
        PW_EMPTY = 512,
        PW_SHORT = 1024,
        PW_LONG = 2048,
        PW_NO_NUM = 4096,
        PW_NO_UPPER = 8192,
        NO_SESSION = 16384
    }
    
    public static readonly IReadOnlyDictionary<StatusCode, string> StatusMessages = new Dictionary<StatusCode, string> {
        { StatusCode.NAME_EMPTY, "User name cannot be empty." },
        { StatusCode.NAME_SHORT, "User name too short." },
        { StatusCode.NAME_LONG, "User name too long." },
        { StatusCode.NAME_WS_PADDED, "User name has whitespace padding." },
        { StatusCode.NAME_WS_CONSEC, "User name has consecutive whitespaces." },
        { StatusCode.NAME_WS_MISC, "User has invalid whitespace characters." },
        { StatusCode.NAME_UNICODE, "User name has unicode characters." },
        { StatusCode.EMAIL_EMPTY, "Email is empty." },
        { StatusCode.EMAIL_MALFORMED, "Email is malformed." },
        { StatusCode.PW_EMPTY, "Password is empty." },
        { StatusCode.PW_SHORT, "Password is too short." },
        { StatusCode.PW_LONG, "Password is too long." },
        { StatusCode.PW_NO_NUM, "Password is missing numbers." },
        { StatusCode.PW_NO_UPPER, "Password missing uppercase letters." },
    }.AsReadOnly();


    public static List<string> GetSubmitStatuses( StatusCode code ) {
        var statuses = new List<string>();

        foreach( (StatusCode msgCode, string message) in StatusMessages ) {
            if( (code & msgCode) != 0 ) {
                statuses.Add( message );
            }
        }

        return statuses;
    }


    public static StatusCode GetUserNameStatus( string userName ) {
        if( string.IsNullOrEmpty(userName) ) {
            return StatusCode.NAME_EMPTY;
        }

        StatusCode code = 0;

        if( userName.Length < 3 ) {
            code |= StatusCode.NAME_SHORT;
        } else if( userName.Length >= 32 ) {
            code |= StatusCode.NAME_LONG;
        }

        if( userName[0] == ' ' || userName[userName.Length-1] == ' ' ) {
            code |= StatusCode.NAME_WS_PADDED;
        }

        int consecSpaces = 0;
        for( int i=0; i<userName.Length; i++ ) {
            char c = userName[i];
            if( c == ' ' ) {
                consecSpaces++;
            } else {
                consecSpaces = 0;
            }
            if( consecSpaces >= 2 ) {
                code |= StatusCode.NAME_WS_CONSEC;
            }
            if( c != ' ' && !Char.IsLetterOrDigit(c) ) {
                code |= StatusCode.NAME_WS_MISC;
            }
            if( c > 127 ) {
                code |= StatusCode.NAME_UNICODE;
            }
        }

        return code;
    }

    public static StatusCode GetEmailStatus( string email ) {
        if( string.IsNullOrEmpty(email) ) {
            return StatusCode.EMAIL_EMPTY;
        }

        return new EmailAddressAttribute().IsValid( email )
            ? StatusCode.OK
            : StatusCode.EMAIL_MALFORMED;
    }

    public static StatusCode GetPasswordStatus( string password ) {
        if( string.IsNullOrEmpty(password) ) {
            return StatusCode.PW_EMPTY;
        }

        StatusCode code = 0;

        if( password.Length < 5 ) {
            code |= StatusCode.PW_SHORT;
        } else if( password.Length >= 1000 ) {
            code |= StatusCode.PW_LONG;
        }

        bool hasNumber = false;
        bool hasUpper = false;

        for( int i=0; i<password.Length; i++ ) {
            char c = password[i];
            hasNumber |= Char.IsDigit( c );
            hasUpper |= Char.IsUpper( c );
        }

        if( !hasNumber ) {
            code |= StatusCode.PW_NO_NUM;
        }
        if( !hasUpper ) {
            code |= StatusCode.PW_NO_UPPER;
        }

        return code;
    }
}
