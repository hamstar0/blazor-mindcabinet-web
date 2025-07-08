using MindCabinet.Shared.Utility;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;


namespace MindCabinet.Shared.DataEntries;


public partial class SimpleUserEntry : IEquatable<SimpleUserEntry> {
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
}
