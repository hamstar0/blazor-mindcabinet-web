using Dapper;
using Konscious.Security.Cryptography;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using System.Data;
using System.Security.Cryptography;
using System.Text;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_SimpleUsers : IServerDataAccess {
    public static byte[] GeneratePasswordSalt() {
        byte[] pwSalt = new byte[ SimpleUserObject.PasswordSaltLength ];

        using( var rng = RandomNumberGenerator.Create() ) {
            rng.GetBytes( pwSalt );
        }

        return pwSalt;
    }

    public static byte[] GeneratePasswordHash( string password, byte[] pwSalt ) {
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



    public const string TableName = "SimpleUsers";

    public async Task<(bool success, long defaultUserId)> Install_Async(
                IDbConnection dbConnection ) {
        await dbConnection.ExecuteAsync( $@"
            CREATE TABLE {TableName} (
                Id BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                Created DATETIME(2) NOT NULL,
                Name VARCHAR(128) NOT NULL,
                Email VARCHAR(320) NOT NULL,
                PwHash BINARY({SimpleUserObject.PasswordHashLength}) NOT NULL,
                PwSalt BINARY({SimpleUserObject.PasswordSaltLength}) NOT NULL,
                IsValidated BOOLEAN NOT NULL
            );"
        //    ON DELETE CASCADE
        //    ON UPDATE CASCADE
        );
        
        byte[] pwSalt = ServerDataAccess_SimpleUsers.GeneratePasswordSalt();
        byte[] pwHash = ServerDataAccess_SimpleUsers.GeneratePasswordHash( "12345", pwSalt );

        long defaultUserId = await dbConnection.ExecuteScalarAsync<long>(   //ExecuteAsync + ExecuteScalarAsync?
            $@"INSERT INTO {TableName} (Created, Name, Email, PwHash, PwSalt, IsValidated) 
                VALUES (@Created, @Name, @Email, @PwHash, @PwSalt, @IsValidated);
            SELECT LAST_INSERT_ID();",
            new {
                Created = DateTime.UtcNow,
                Name = "hamstar",   // temporary!!!!!
                Email = "hamstarhelper@gmail.com",
                PwHash = pwHash,
                PwSalt = pwSalt,
                IsValidated = true,
            }
        );
        //throw new Exception( JsonSerializer.Serialize(obj) );

        return (true, defaultUserId);
    }

    //

    private ServerSessionData SessionData;
    
    private ServerSettings ServerSettings;



    public ServerDataAccess_SimpleUsers( ServerSessionData sessionData, ServerSettings serverSettings ) {
        this.SessionData = sessionData;
        this.ServerSettings = serverSettings;
    }

    private IDictionary<long, SimpleUserObject> SimpleUsersById_Cache = new Dictionary<long, SimpleUserObject>();



    public async Task<SimpleUserObject?> GetSimpleUser_Async( IDbConnection dbCon, long id ) {
        if( this.SimpleUsersById_Cache.ContainsKey( id ) ) {
            return this.SimpleUsersById_Cache[id];
        }

        SimpleUserObject.User_DatabaseEntry? userRaw = await dbCon.QuerySingleAsync<SimpleUserObject.User_DatabaseEntry?>(
            $"SELECT * FROM {TableName} WHERE Id = @Id",
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
        SimpleUserObject.User_DatabaseEntry? userRaw = await dbCon.QuerySingleAsync<SimpleUserObject.User_DatabaseEntry?>(
            $"SELECT * FROM {TableName} WHERE Name = @Name",
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
        var userRaw = await dbCon.QuerySingleOrDefaultAsync<SimpleUserObject.UserAndSession_DatabaseEntry?>(
            $@"SELECT * FROM {TableName} AS MyUsers
                INNER JOIN {ServerDataAccess_SimpleUsers_Sessions.TableName} AS MySessions
                    ON (MyUsers.Id = MySessions.SimpleUserId) 
                WHERE MySessions.SessionId = @SessionId",
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
                $"DELETE FROM {ServerDataAccess_SimpleUsers_Sessions.TableName} WHERE Id = @SessionId",
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
                ClientDataAccess_SimpleUsers.Create_Params parameters ) {
        var userByName = await dbCon.QuerySingleAsync<SimpleUserObject.User_DatabaseEntry?>(
            $"SELECT * FROM {TableName} WHERE Name = @Name",
            new { Name = parameters.Name }
        );
        if( userByName is not null ) {
            return new SimpleUserQueryResult( null, $"User {parameters.Name} already exists" );
        }

        DateTime now = DateTime.UtcNow;

        byte[] pwSalt = ServerDataAccess_SimpleUsers.GeneratePasswordSalt();
        byte[] pwHash = ServerDataAccess_SimpleUsers.GeneratePasswordHash( parameters.Password, pwSalt );

        long newUserId = await dbCon.ExecuteScalarAsync<long>(   //QuerySingleAsync
            $@"INSERT INTO {TableName} (Created, Name, Email, PwHash, PwSalt, IsValidated) 
                VALUES (@Created, @Name, @Email, @PwHash, @PwSalt, @IsValidated);
            SELECT LAST_INSERT_ID();",  //OUTPUT INSERTED.Id 
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
