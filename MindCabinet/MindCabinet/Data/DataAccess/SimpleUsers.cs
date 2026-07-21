using Dapper;
using Konscious.Security.Cryptography;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Client.Site.Pages;
using MindCabinet.DataObjects;
using MindCabinet.Services;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.PostsContext;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.Utility;
using System.Data;
using System.Security.Cryptography;
using System.Text;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_SimpleUsers : IServerDataAccess {
    private static readonly SimpleCache<SimpleUserId, SimpleUserObject.Raw?> Cache_ById = new( refreshExpiryOnGet: true );
    private static readonly SimpleCache<string, SimpleUserId?> Cache_ByName = new( refreshExpiryOnGet: true );



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
    


    public class SimpleUserQueryResult(
                SimpleUserObject.Raw? user,
                PostsContextObject.Raw? defaultPostsContext,
                TermObject.Raw? asTerm, bool alreadyExists ) {
        public SimpleUserObject.Raw? User = user;
        public PostsContextObject.Raw? UserDefaultPostsContextId = defaultPostsContext;
        public TermObject.Raw? UserAsTermId = asTerm;
        public bool AlreadyExists = alreadyExists;
    }



    private readonly Services.ClientSessionManager SessionManager;

    private readonly StaticServerSettings ServerSettings;

    private readonly ILogger<ServerDataAccess_SimpleUsers> Logger;



    public ServerDataAccess_SimpleUsers(
				Services.ClientSessionManager sessionMngr,
                StaticServerSettings serverSettings,
                ILogger<ServerDataAccess_SimpleUsers> logger ) {
        this.SessionManager = sessionMngr;
        this.ServerSettings = serverSettings;
        this.Logger = logger;
    }



    public async Task<SimpleUserObject.Raw?> GetById_Async( IDbConnection dbCon, SimpleUserId id ) {
        if( id == 0 ) {
            throw new ArgumentException( "SimpleUserId is not valid (must be non-zero)." );
        }

        //

        if( ServerDataAccess_SimpleUsers.Cache_ById.TryGet(id, out var cached) ) {
            return cached;
        }

        //

        SimpleUserObject.Raw? userRaw = await dbCon.QuerySingleOrDefaultAsync<SimpleUserObject.Raw>(
            $"SELECT * FROM {TableName} WHERE {TableColumn_Id} = @Id",
            new { Id = (long)id }
        );

        //

        ServerDataAccess_SimpleUsers.Cache_ById.Set(
            key: id,
            value: userRaw,
            expiry: this.ServerSettings.CacheExpirationDuration
        );
        if( userRaw is not null ) {
            ServerDataAccess_SimpleUsers.Cache_ByName.Set(
                key: userRaw.Name,
                value: userRaw.Id,
                expiry: this.ServerSettings.CacheExpirationDuration
            );
        }

        //

        return userRaw;
    }


    public async Task<SimpleUserObject.Raw?> GetByName_Async( IDbConnection dbCon, string userName ) {
        if( SimpleUserObject.GetUserNameStatus(userName) != SimpleUserObject.StatusCode.OK ) {
            throw new ArgumentException( "Invalid user name." );
        }

        //

        if( ServerDataAccess_SimpleUsers.Cache_ByName.TryGet(userName, out var cachedId) && cachedId is not null ) {
            if( ServerDataAccess_SimpleUsers.Cache_ById.TryGet(cachedId.Value, out var cachedUser) ) {
                return cachedUser;
            }
        }

        //
        
        SimpleUserObject.Raw? userRaw = await dbCon.QuerySingleOrDefaultAsync<SimpleUserObject.Raw>(
            $"SELECT * FROM {TableName} WHERE {TableColumn_Name} = @Name",
            new { Name = userName }
        );

        //

        ServerDataAccess_SimpleUsers.Cache_ByName.Set(
            key: userName,
            value: userRaw?.Id,
            expiry: this.ServerSettings.CacheExpirationDuration
        );
        if( userRaw is not null ) {
            ServerDataAccess_SimpleUsers.Cache_ById.Set(
                key: userRaw.Id,
                value: userRaw,
                expiry: this.ServerSettings.CacheExpirationDuration
            );
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
                    MySessions.{ServerDataAccess_SimpleUserSessions.TableColumn_Id} AS SessionId,
                    MySessions.{ServerDataAccess_SimpleUserSessions.TableColumn_LatestIpAddress} AS LatestIpAddress,
                    MySessions.{ServerDataAccess_SimpleUserSessions.TableColumn_SimpleUserId} AS SimpleUserId,
                    MySessions.{ServerDataAccess_SimpleUserSessions.TableColumn_FirstVisit} AS FirstVisit,
                    MySessions.{ServerDataAccess_SimpleUserSessions.TableColumn_LatestVisit} AS LatestVisit,
                    MySessions.{ServerDataAccess_SimpleUserSessions.TableColumn_Visits} AS Visits
                FROM {TableName} AS MyUsers
                INNER JOIN {ServerDataAccess_SimpleUserSessions.TableName} AS MySessions
                    ON (MyUsers.{TableColumn_Id} = MySessions.{ServerDataAccess_SimpleUserSessions.TableColumn_SimpleUserId}) 
                WHERE MySessions.{ServerDataAccess_SimpleUserSessions.TableColumn_Id} = @SessionId",
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
                    $"DELETE FROM {ServerDataAccess_SimpleUserSessions.TableName} WHERE {ServerDataAccess_SimpleUserSessions.TableColumn_Id} = @Id",
                    new {
                        Id = sessionId
                    }
                );

                return null;
            }
        }

        //

        SimpleUserObject.Raw userRaw = userAndSessRaw.GetUserRaw();

        ServerDataAccess_SimpleUsers.Cache_ById.Set(
            key: userRaw.Id,
            value: userRaw,
            expiry: this.ServerSettings.CacheExpirationDuration
        );
        ServerDataAccess_SimpleUsers.Cache_ByName.Set(
            key: userRaw.Name,
            value: userRaw.Id,
            expiry: this.ServerSettings.CacheExpirationDuration
        );

        return userRaw;
    }
}
