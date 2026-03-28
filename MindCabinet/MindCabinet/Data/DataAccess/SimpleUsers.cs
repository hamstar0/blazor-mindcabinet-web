using Dapper;
using Konscious.Security.Cryptography;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Client.Site.Pages;
using MindCabinet.DataObjects;
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



    public class SimpleUserQueryResult( SimpleUserObject.Raw? user, bool alreadyExists ) {
        public SimpleUserObject.Raw? User = user;
        public bool AlreadyExists = alreadyExists;
    }

    //



    public const string TableName = "SimpleUsers";

    public async Task<(bool success, SimpleUserId defaultUserId)> Install_Async(
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
        
        SimpleUserQueryResult result = await this.CreateSimpleUser_Async(
            dbCon: dbConnection,
            parameters: new ClientDataAccess_SimpleUsers.Create_Params(
                name: "hamstar",   // temporary!!!!!
                email: "hamstarhelper@gmail.com",
                password: "12345A",
                isValidated: true
            ),
            detectCollision: false
        );
        //throw new Exception( JsonSerializer.Serialize(obj) );

        if( result.User is null ) {
            throw new Exception( "Failed to create default user: "+(result.AlreadyExists ? "already exists" : "unknown error") );
        }

        return (true, result.User.Id);
    }

    //

    private readonly ServerSessionData SessionData;
    
    private readonly ServerSettings ServerSettings;

    private readonly ILogger<ServerDataAccess_SimpleUsers> Logger;



    public ServerDataAccess_SimpleUsers(
                ServerSessionData sessionData,
                ServerSettings serverSettings,
                ILogger<ServerDataAccess_SimpleUsers> logger ) {
        this.SessionData = sessionData;
        this.ServerSettings = serverSettings;
        this.Logger = logger;
    }

    private IDictionary<SimpleUserId, SimpleUserObject.Raw> SimpleUsersById_Cache = new Dictionary<SimpleUserId, SimpleUserObject.Raw>();



    public async Task<SimpleUserObject.Raw?> GetById_Async( IDbConnection dbCon, SimpleUserId id ) {
        if( id == 0 ) {
            throw new ArgumentException( "SimpleUserId is not valid (must be non-zero)." );
        }

        if( this.SimpleUsersById_Cache.ContainsKey( id ) ) {
            return this.SimpleUsersById_Cache[id];
        }

        SimpleUserObject.Raw? userRaw = await dbCon.QuerySingleOrDefaultAsync<SimpleUserObject.Raw>(
            $"SELECT * FROM {TableName} WHERE Id = @Id",
            new { Id = (long)id }
        );

        if( userRaw is not null ) {
            this.SimpleUsersById_Cache.Add( id, userRaw );
        }

        return userRaw;
    }


    public async Task<SimpleUserObject.Raw?> GetByName_Async( IDbConnection dbCon, string userName ) {
        if( SimpleUserObject.GetUserNameStatus(userName) != SimpleUserObject.StatusCode.OK ) {
            throw new ArgumentException( "Invalid user name." );
        }
        
        SimpleUserObject.Raw? userRaw = await dbCon.QuerySingleOrDefaultAsync<SimpleUserObject.Raw>(
            $"SELECT * FROM {TableName} WHERE Name = @Name",
            new { Name = userName }
        );

        if( userRaw is not null ) {
            this.SimpleUsersById_Cache[ userRaw.Id ] = userRaw;
        }

        return userRaw;
    }

    public async Task<SimpleUserObject.Raw?> GetSimpleUserBySession_Async(
                IDbConnection dbCon,
                string sessionId,
                string ipAddress,
                bool destroyIfSessionExpiredOrInvalid ) {
        if( !UserSessionObject.ValidateId(sessionId) ) {
            throw new ArgumentException( $"Invalid session ID ({sessionId})." );
        }
        if( !UserSessionObject.ValidateIpAddress(ipAddress) ) {
            throw new ArgumentException( $"Invalid IP address ({ipAddress})." );
        }
        
        var userAndSessRaw = await dbCon.QuerySingleOrDefaultAsync<SimpleUserObject.UserAndSession_Raw>(  // Note: MySessions.Id is SessionId to avoid collision
            $@"SELECT
                    MyUsers.*,
                    MySessions.Id AS SessionId,
                    MySessions.LatestIpAddress AS LatestIpAddress,
                    MySessions.SimpleUserId AS SimpleUserId,
                    MySessions.FirstVisit AS FirstVisit,
                    MySessions.LatestVisit AS LatestVisit,
                    MySessions.Visits AS Visits
                FROM {TableName} AS MyUsers
                INNER JOIN {ServerDataAccess_SimpleUserSessions.TableName} AS MySessions
                    ON (MyUsers.Id = MySessions.SimpleUserId) 
                WHERE MySessions.Id = @SessionId",
            new { SessionId = sessionId }
        );

        if( userAndSessRaw is null ) {
            return null;
        }

        if( destroyIfSessionExpiredOrInvalid ) {
            bool isValidIp = userAndSessRaw.LatestIpAddress == ipAddress;
            bool isExpired = (DateTime.UtcNow - userAndSessRaw.LatestVisit) > this.ServerSettings.SessionExpirationDuration;

            if( !isValidIp ) {
                throw new Exception( "Hax!" );  //TODO
            }

            if( !isValidIp || isExpired ) {
                // SimpleUserObject.UserAndSession_DatabaseEntry? entry =
                //     await dbCon.QuerySingleOrDefaultAsync<SimpleUserObject.UserAndSession_DatabaseEntry>(
                //         $@"SELECT MySessions.Id FROM {ServerDataAccess_SimpleUsers_Sessions.TableName} AS MySessions
                //             WHERE MySessions.Id = @Id",
                //         new { Id = sessionId }
                //     );
                // if( entry is not null ) {
                await dbCon.ExecuteAsync(
                    $"DELETE FROM {ServerDataAccess_SimpleUserSessions.TableName} WHERE Id = @Id",
                    new {
                        Id = sessionId
                    }
                );

                return null;
            }
        }

        SimpleUserObject.Raw userRaw = userAndSessRaw.GetUserRaw();

        this.SimpleUsersById_Cache.Add( userAndSessRaw.Id, userRaw );

        return userRaw;
    }


    public async Task<SimpleUserQueryResult> CreateSimpleUser_Async(
                IDbConnection dbCon,
                ClientDataAccess_SimpleUsers.Create_Params parameters,
                bool detectCollision ) {
        SimpleUserObject.StatusCode code;
        code = SimpleUserObject.GetUserNameStatus(parameters.Name);
        if( code != SimpleUserObject.StatusCode.OK ) {
            throw new ArgumentException( $"User name is not valid ({code.ToString()})." );
        }
        code = SimpleUserObject.GetEmailStatus(parameters.Email);
        if( code != SimpleUserObject.StatusCode.OK ) {
            throw new ArgumentException( $"Email is not valid ({code.ToString()})." );
        }
        code = SimpleUserObject.GetPasswordStatus(parameters.Password);
        if( code != SimpleUserObject.StatusCode.OK ) {
            throw new ArgumentException( $"Password is not valid ({code.ToString()})." );
        }

        SimpleUserObject.Raw? user;

        if( detectCollision ) {
            user = await dbCon.QuerySingleOrDefaultAsync<SimpleUserObject.Raw?>(
                $"SELECT * FROM {TableName} WHERE Name = @Name",
                new { Name = parameters.Name }
            );
            if( user is not null ) {
                return new SimpleUserQueryResult( null, true );
            }
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
                IsValidated = parameters.IsValidated
            }
        );

        var newUser = SimpleUserObject.CreateRaw(
            id: (SimpleUserId)newUserId,
            created: now,
            name: parameters.Name,
            email: parameters.Email,
            pwHash: pwHash,
            pwSalt: pwSalt,
            isValidated: parameters.IsValidated    // note: not for client
            //isPrivileged: isPrivileged
        );

        return new SimpleUserQueryResult( newUser, false );
    }
}
