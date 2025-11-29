using Dapper;
using Konscious.Security.Cryptography;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using System.Data;
using System.Text;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_SimpleUsers {
    public static byte[] GetPasswordHash( string password, byte[] pwSalt ) {
        var argon2id = new Argon2id( Encoding.UTF8.GetBytes( password ) );
        argon2id.Salt = pwSalt;
        argon2id.MemorySize = 12288;
        argon2id.Iterations = 2;
        argon2id.DegreeOfParallelism = 1;

        return argon2id.GetBytes( SimpleUserObject.PasswordHashLength );
    }

    //



    public class SimpleUserQueryResult( SimpleUserObject? user, string status ) {
        public SimpleUserObject? User = user;
        public string Status = status;
    }

    //



    public async Task<(bool success, long defaultUserId)> Install_Async(
                IDbConnection dbConnection ) {
        await dbConnection.ExecuteAsync( @"
            CREATE TABLE SimpleUsers (
                Id BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                Created DATETIME(2) NOT NULL,
                Name VARCHAR(128) NOT NULL,
                Email VARCHAR(320) NOT NULL,
                PwHash BINARY("+SimpleUserObject.PasswordHashLength+@") NOT NULL,
                PwSalt BINARY("+SimpleUserObject.PasswordSaltLength+@") NOT NULL,
                IsValidated BOOLEAN NOT NULL
            );"
        //    ON DELETE CASCADE
        //    ON UPDATE CASCADE
        );
        
        long defaultUserId = await dbConnection.ExecuteScalarAsync<long>(   //ExecuteAsync + ExecuteScalarAsync?
            @"INSERT INTO SimpleUsers (Created, Name, Email, PwHash, PwSalt, IsValidated) 
                VALUES (@Created, @Name, @Email, @PwHash, @PwSalt, @IsValidated);
            SELECT LAST_INSERT_ID();",
            new {
                Created = DateTime.UtcNow,
                Name = "hamstar",
                Email = "hamstarhelper@gmail.com",
                PwHash = new byte[SimpleUserObject.PasswordHashLength],
                PwSalt = new byte[SimpleUserObject.PasswordSaltLength],
                IsValidated = false,
            }
        );
        //throw new Exception( JsonSerializer.Serialize(obj) );

        return (true, defaultUserId);
    }

    //

    private ServerSettings ServerSettings;



    public ServerDataAccess_SimpleUsers( ServerSettings serverSettings ) {
        this.ServerSettings = serverSettings;
    }

    private IDictionary<long, SimpleUserObject> SimpleUsersById_Cache = new Dictionary<long, SimpleUserObject>();



    public async Task<SimpleUserObject?> GetSimpleUser_Async( IDbConnection dbCon, long id ) {
        if( this.SimpleUsersById_Cache.ContainsKey( id ) ) {
            return this.SimpleUsersById_Cache[id];
        }

        SimpleUserObject.UserDbData? userRaw = await dbCon.QuerySingleAsync<SimpleUserObject.UserDbData?>(
            "SELECT * FROM SimpleUsers WHERE Id = @Id",
            new { Id = id }
        );

        if( userRaw is null ) {
            return null;
        }

        SimpleUserObject user = userRaw.CreateUserEntry();

        this.SimpleUsersById_Cache.Add( id, user );

        return user;
    }


    public async Task<SimpleUserObject?> GetSimpleUser_Async( IDbConnection dbCon, string userName ) {
        SimpleUserObject.UserDbData? userRaw = await dbCon.QuerySingleAsync<SimpleUserObject.UserDbData?>(
            "SELECT * FROM SimpleUsers WHERE Name = @Name",
            new { Name = userName }
        );

        if( userRaw is null ) {
            return null;
        }

        SimpleUserObject user = userRaw.CreateUserEntry();

        this.SimpleUsersById_Cache.Add( userRaw.Id, user );

        return user;
    }

    public async Task<SimpleUserObject?> GetSimpleUserBySession_Async(
                IDbConnection dbCon,
                string sessionId,
                string ipAddress ) {
        var userRaw = await dbCon.QuerySingleOrDefaultAsync<SimpleUserObject.UserAndSessionDbData?>(
            @"SELECT * FROM SimpleUsers AS Users 
                INNER JOIN SimpleUserSessions AS Sessions ON (Users.Id = Sessions.SimpleUserId) 
                WHERE Sessions.SessionId = @SessionId",
            new { SessionId = sessionId }
        );

        if( userRaw is null ) {
            return null;
        }

        bool isValidIp = userRaw.IpAddress == ipAddress;
        bool isNotExpired = (DateTime.UtcNow - userRaw.LatestVisit) > this.ServerSettings.SessionExpirationDuration;

        if( !isValidIp ) {
            throw new Exception( "Hax!" );  //TODO
        }

        if( !isValidIp || !isNotExpired ) {
            await dbCon.ExecuteAsync(
                "DELETE FROM SimpleUserSessions WHERE Id = @SessionId",
                new {
                    SessionId = sessionId
                }
            );

            return null;
        }

        SimpleUserObject user = userRaw.CreateUserEntry();

        this.SimpleUsersById_Cache.Add( (long)user.Id!, user );

        return user;
    }


    public async Task<SimpleUserQueryResult> CreateSimpleUser_Async(
                IDbConnection dbCon,
                ClientDataAccess_SimpleUsers.Create_Params parameters,
                byte[] pwSalt ) {
        var userByName = await dbCon.QuerySingleAsync<SimpleUserObject.UserDbData?>(
            "SELECT * FROM SimpleUsers WHERE Name = @Name",
            new { Name = parameters.Name }
        );
        if( userByName is not null ) {
            return new SimpleUserQueryResult( null, $"User {parameters.Name} already exists" );
        }

        DateTime now = DateTime.UtcNow;

        byte[] pwHash = ServerDataAccess_SimpleUsers.GetPasswordHash( parameters.Password, pwSalt );

        int newUserId = await dbCon.QuerySingleAsync(
            @"INSERT INTO SimpleUsers (Created, Name, Email, PwHash, PwSalt, IsValidated) 
                OUTPUT INSERTED.Id 
                VALUES (@Created, @Name, @Email, @PwHash, @PwSalt, @IsValidated);",
            new {
                Created = now,
                Name = parameters.Name,
                Email = parameters.Email,
                PwHash = pwHash,
                PwSalt = pwSalt,
                IsValidated = false
            }
        );

        var newUser = new SimpleUserObject(
            id: newUserId,
            created: now,
            name: parameters.Name,
            email: parameters.Email,
            pwHash: pwHash,
            pwSalt: pwSalt,
            isValidated: false
        );

        return new SimpleUserQueryResult( newUser, $"User {parameters.Name} successfully created." );
    }
}
