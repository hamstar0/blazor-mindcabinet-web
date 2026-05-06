using Dapper;
using MindCabinet.DataObjects;
using MindCabinet.Services;
using MindCabinet.Shared.DataObjects;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_SimpleUserSessions : IServerDataAccess {
    public async Task<UserSessionObject.Raw?> GetById_Async(
                IDbConnection dbCon,
                string sessionId ) {
        if( UserSessionObject.ValidateId(sessionId) ) {
            throw new ArgumentException( "UserSession Id is not valid." );
        }

        UserSessionObject.Raw? sessionData = await dbCon.QuerySingleOrDefaultAsync<UserSessionObject.Raw>(
            $@"SELECT * FROM {TableName} WHERE {TableColumn_Id} = @Id",
            new { Id = sessionId }
        );

        return sessionData;
    }
    
    public async Task Create_Async(
                IDbConnection dbCon,
                SimpleUserId simpleUserId,
                ServerSessionManager sessionMngr ) {
        if( !sessionMngr.IsLoaded ) {
            throw new Exception( "Session not loaded." );
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
    }
}
