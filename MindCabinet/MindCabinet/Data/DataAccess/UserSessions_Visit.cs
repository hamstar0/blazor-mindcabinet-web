using Dapper;
using MindCabinet.DataObjects;
using MindCabinet.Services;
using MindCabinet.Shared.DataObjects;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_SimpleUserSessions : IServerDataAccess {
        public async Task VisitSimpleUserSession_Async(
                IDbConnection dbCon,
                ServerSessionManager sessMngr ) {
        if( !sessMngr.IsLoaded ) {
            throw new Exception( "Session not loaded." );
        }

        int rows = await dbCon.ExecuteAsync(
            $@"UPDATE {TableName}
                SET Visits = Visits + 1, LatestVisit = @Now
                WHERE Id = @Id",
            new {
                Now = DateTime.UtcNow,
                Id = sessMngr.CurrentSessionId
            }
        );
        if( rows == 0 ) {
            throw new Exception( "No session found to indicate a visit." );
        }
    }
}
