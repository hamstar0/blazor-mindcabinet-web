using Dapper;
using MindCabinet.DataObjects;
using MindCabinet.Services;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.Utility;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_SimpleUserSessions(
                StaticServerSettings serverSettings
            ) : IServerDataAccess {
    private static readonly SimpleCache<string, UserSessionObject.Raw?> Cache_BySessionId = new( refreshOnGet: true );



    private readonly StaticServerSettings ServerSettings = serverSettings;



    public async Task<UserSessionObject.Raw?> GetById_Async(
                IDbConnection dbCon,
                string sessionId ) {
        if( UserSessionObject.ValidateId(sessionId) ) {
            throw new ArgumentException( "UserSession Id is not valid." );
        }

        //

        if( ServerDataAccess_SimpleUserSessions.Cache_BySessionId.TryGet(sessionId, out var cached) ) {
            return cached;
        }

        //

        UserSessionObject.Raw? sessionData = await dbCon.QuerySingleOrDefaultAsync<UserSessionObject.Raw>(
            $@"SELECT * FROM {TableName} WHERE {TableColumn_Id} = @Id",
            new { Id = sessionId }
        );

        //

        ServerDataAccess_SimpleUserSessions.Cache_BySessionId.Set(
            key: sessionId,
            value: sessionData,
            expiry: this.ServerSettings.CacheExpirationDuration
        );

        //

        return sessionData;
    }
    
    public async Task<UserSessionObject.Raw> Create_Async(
                IDbConnection dbCon,
                SimpleUserId simpleUserId,
                ClientSessionManager sessionMngr ) {
        if( !sessionMngr.IsLoaded ) {
            throw new Exception( "Session not loaded." );
        }
        if( sessionMngr.CurrentSessionId is null ) {
            throw new Exception( "Session ID is null." );
        }
        if( simpleUserId == 0 ) {
            throw new ArgumentException( "SimpleUserId is not valid (must be non-zero)." );
        }
        if( sessionMngr.CurrentIpAddress is null ) {
            throw new Exception( "Invalid IP address." );
        }

        //var sessData = await dbCon.QuerySingleAsync<SimpleUserEntry.SessionDbData?>(
        //    "SELECT * FROM SimpleUserSession WHERE SessionId = @SessionId",
        //    new { SessionId = session.CurrentSessionId }
        //);
        //if( sessData is not null ) {
        //    throw new Exception( "Session already exists." );
        //}

        DateTime now = DateTime.UtcNow;

        int rows = await dbCon.ExecuteAsync(
            $@"INSERT INTO {TableName} (
                    {TableColumn_Id},
                    {TableColumn_LatestIpAddress},
                    {TableColumn_SimpleUserId},
                    {TableColumn_FirstVisit},
                    {TableColumn_LatestVisit},
                    {TableColumn_Visits}) 
                VALUES (@Id, @LatestIpAddress, @SimpleUserId, @FirstVisit, @LatestVisit, @Visits)",
            new {
                Id = sessionMngr.CurrentSessionId,
                LatestIpAddress = sessionMngr.CurrentIpAddress,
                SimpleUserId = (long)simpleUserId,
                FirstVisit = now,
                LatestVisit = now,
                Visits = 1
            }
        );
        if( rows != 1 ) {
            throw new Exception( "Session already exists." );
        }

        var sessionData = new UserSessionObject.Raw {
            Id = sessionMngr.CurrentSessionId,
            LatestIpAddress = sessionMngr.CurrentIpAddress,
            SimpleUserId = simpleUserId,
            FirstVisit = now,
            LatestVisit = now,
            Visits = 1
         };

        //

        ServerDataAccess_SimpleUserSessions.Cache_BySessionId.Set(
            key: sessionMngr.CurrentSessionId,
            value: sessionData,
            expiry: this.ServerSettings.CacheExpirationDuration
        );

        //

        return sessionData;
    }


    public async Task RemoveSessionById_Async( IDbConnection dbCon, string sessionId ) {
        if( !UserSessionObject.ValidateId(sessionId) ) {
            throw new ArgumentException( $"UserSession Id is not valid." );
        }
        
        int rows = await dbCon.ExecuteAsync(
            $@"DELETE FROM {TableName} WHERE {TableColumn_Id} = @Id",
            new {
                Id = sessionId
            }
        );
        if( rows == 0 ) {
            throw new Exception( "No session found to remove." );
        }

        //

        ServerDataAccess_SimpleUserSessions.Cache_BySessionId.Remove( sessionId );
    }
}
