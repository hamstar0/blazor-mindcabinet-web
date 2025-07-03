using Dapper;
using Konscious.Security.Cryptography;
using MindCabinet.Client.Services;
using MindCabinet.Shared.DataEntries;
using System.Data;
using System.Text;


namespace MindCabinet.Data;


public partial class ServerDbAccess {
    public class SimpleUserQueryResult( SimpleUserEntry? user, string status ) {
        public SimpleUserEntry? User = user;
        public string Status = status;
    }

    public class SimpleUserEntryData {
        public long Id;
        public DateTime Created;
        public string Name = "";
        public string Email = "";
        public byte[] PwHash = new byte[SimpleUserEntry.PasswordHashLength];
        public byte[] PwSalt = new byte[SimpleUserEntry.PasswordSaltLength];
        public bool IsValidated = false;
        //public bool IsPrivileged = false;
        

        public SimpleUserEntry Create( IDbConnection dbCon, ServerDbAccess data ) {
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

    //



    public async Task<bool> InstallSimpleUsers_Async( IDbConnection dbConnection ) {
        await dbConnection.ExecuteAsync( @"
            CREATE TABLE SimpleUsers (
                Id BIGINT NOT NULL IDENTITY(1, 1) PRIMARY KEY CLUSTERED,
                Created DATETIME2(2) NOT NULL,
                Name VARCHAR(128) NOT NULL,
                Email VARCHAR(320) NOT NULL,
                PwHash BYTE("+SimpleUserEntry.PasswordHashLength+@") NOT NULL,
                PwSalt BYTE("+SimpleUserEntry.PasswordSaltLength+@") NOT NULL,
                IsValidated BIT NOT NULL
            );"
        //    ON DELETE CASCADE
        //    ON UPDATE CASCADE
        );

        return true;
    }

    //



    private IDictionary<long, SimpleUserEntry> SimpleUsersById_Cache = new Dictionary<long, SimpleUserEntry>();



    public async Task<SimpleUserEntry?> GetSimpleUser_Async( IDbConnection dbCon, long id ) {
        if( this.SimpleUsersById_Cache.ContainsKey( id ) ) {
            return this.SimpleUsersById_Cache[id];
        }

        SimpleUserEntryData? userRaw = await dbCon.QuerySingleAsync<SimpleUserEntryData?>(
            "SELECT * FROM SimpleUsers AS MyUser WHERE Id = @Id",
            new { Id = id }
        );

        if( userRaw is null ) {
            return null;
        }

        SimpleUserEntry user = userRaw.Create( dbCon, this );

        this.SimpleUsersById_Cache.Add( id, user );

        return user;
    }


    public async Task<SimpleUserQueryResult> CreateSimpleUser_Async(
                IDbConnection dbCon,
                ClientDbAccess.CreateSimpleUserParams parameters,
                byte[] pwSalt ) {
        SimpleUserEntryData? userByName = await dbCon.QuerySingleAsync<SimpleUserEntryData?>(
            @"SELECT * FROM SimpleUsers AS MyUser
                WHERE Name = @Name",
            new { Name = parameters.Name }
        );
        if( userByName is not null ) {
            return new SimpleUserQueryResult( null, $"User {parameters.Name} already exists" );
        }

        DateTime now = DateTime.UtcNow;

        var argon2id = new Argon2id( Encoding.UTF8.GetBytes(parameters.Password) );
        argon2id.Salt = pwSalt;
        argon2id.MemorySize = 12288;
        argon2id.Iterations = 2;
        argon2id.DegreeOfParallelism = 1;

        byte[] pw = argon2id.GetBytes( SimpleUserEntry.PasswordHashLength );

        int newUserId = await dbCon.QuerySingleAsync(
            @"INSERT INTO SimpleUsers (Created, Name, Email, PwHash, PwSalt, IsValidated) 
                VALUES (@Created, @Name, @Email, @PwHash, @PwSalt, @IsValidated)
                OUTPUT INSERTED.Id",
            new {
                Created = now,
                Name = parameters.Name,
                Email = parameters.Email,
                PwHash = pw,
                PwSalt = pwSalt,
                IsValidated = false
            }
        );

        var newUser = new SimpleUserEntry(
            id: newUserId,
            created: now,
            name: parameters.Name,
            email: parameters.Email,
            pwHash: pw,
            pwSalt: pwSalt,
            isValidated: false
        );

        return new SimpleUserQueryResult( newUser, $"User {parameters.Name} successfully created." );
    }
}
