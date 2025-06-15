using Dapper;
using MindCabinet.Client.Services;
using MindCabinet.Shared.DataEntries;
using System.Data;


namespace MindCabinet.Data;


public partial class ServerDbAccess {
    public class SimpleUserEntryData {
        public long Id;
        public DateTime Created;
        public string Name = "";
        public string Email = "";
        public string PwHash = "";
        public string PwSalt = "";
        public bool IsValidated = false;
        //public bool IsPrivileged = false;


        public async Task<SimpleUserEntry> Create_Async(
                IDbConnection dbCon,
                ServerDbAccess data ) {
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
                PwHash CHAR(256) NOT NULL,
                PwSalt CHAR(32) NOT NULL,
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
            "SELECT * FROM User AS MyUser WHERE Id = @Id",
            new { Id = id }
        );

        if( userRaw is null ) {
            return null;
        }

        SimpleUserEntry user = await userRaw.Create_Async( dbCon, this );

        this.SimpleUsersById_Cache.Add( id, user );

        return user;
    }


    public async Task<SimpleUserEntry> CreateSimpleUser_Async(
                IDbConnection dbCon,
                ClientDbAccess.CreateSimpleUserParams parameters,
                string pwSalt ) {
        DateTime now = DateTime.UtcNow;

        int newUserId = await dbCon.QuerySingleAsync(
            @"INSERT INTO SimpleUsers (Created, Name, Email, PwHash, PwSalt, IsValidated) 
                VALUES (@Created, @Name, @Email, @PwHash, @PwSalt, @IsValidated)
                OUTPUT INSERTED.Id",
            new {
                Created = now,
                Name = parameters.Name,
                Email = parameters.Email,
                PwHash = parameters.PwHash,
                PwSalt = pwSalt,
                IsValidated = false
            }
        );

        var newUser = new SimpleUserEntry(
            id: newUserId,
            created: now,
            name: parameters.Name,
            email: parameters.Email,
            pwHash: parameters.PwHash,
            pwSalt: pwSalt,
            isValidated: false
        );

        return newUser;
    }
}
