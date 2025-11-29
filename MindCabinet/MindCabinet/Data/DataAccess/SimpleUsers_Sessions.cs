using Dapper;
using MindCabinet.Shared.DataObjects;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_SimpleUsers_Sessions {
    public async Task<bool> Install_Async( IDbConnection dbConnection ) {
        await dbConnection.ExecuteAsync( @"
            CREATE TABLE SimpleUserSessions (
                SessionId VARCHAR(36) NOT NULL,
                IpAddress VARCHAR(45) NOT NULL,
                SimpleUserId BIGINT NOT NULL,
                FirstVisit DATETIME(2) NOT NULL,
                LatestVisit DATETIME(2) NOT NULL,
                Visits INT NOT NULL,
                PRIMARY KEY (SessionId),
                CONSTRAINT FK_SessUserId FOREIGN KEY (SimpleUserId)
                    REFERENCES SimpleUsers(Id)
            );"
        //    ON DELETE CASCADE
        //    ON UPDATE CASCADE
        );

        return true;
    }


    public async Task CreateSimpleUserSession_Async(
                IDbConnection dbCon,
                SimpleUserObject user,
                ServerSessionData session ) {
        if( !session.IsLoaded ) {
            throw new Exception( "Session not loaded." );
        }
        if( session.IpAddress is null ) {
            throw new Exception( "Invalid IP address." );
        }

        //var sessData = await dbCon.QuerySingleAsync<SimpleUserEntry.SessionDbData?>(
        //    "SELECT * FROM SimpleUserSession WHERE SessionId = @SessionId",
        //    new { SessionId = session.SessionId }
        //);
        //if( sessData is not null ) {
        //    throw new Exception( "Session already exists." );
        //}

        DateTime now = DateTime.UtcNow;

        int rows = await dbCon.ExecuteAsync(
            @"INSERT INTO SimpleUserSessions
                (SessionId, IpAddress, SimpleUserId, FirstVisit, LatestVisit, Visits) 
                VALUES (@SessionId, @IpAddress, @SimpleUserId, @FirstVisit, @LatestVisit, @Visits)",
            new {
                SessionId = session.SessionId,
                IpAddress = session.IpAddress,
                SimpleUserId = user.Id,
                FirstVisit = now,
                LatestVisit = now,
                Visits = 1
            }
        );
        if( rows != 1 ) {
            throw new Exception( "Session already exists." );
        }
    }


    public async Task VisitSimpleUserSession_Async(
                IDbConnection dbCon,
                ServerSessionData session ) {
        if( !session.IsLoaded ) {
            throw new Exception( "Session not loaded." );
        }

        int rows = await dbCon.ExecuteAsync(
            @"UPDATE SimpleUserSessions
                SET Visits = Visits + 1, LatestVisit = @Now
                WHERE SessionId = @SessionId",
            new {
                Now = DateTime.UtcNow,
                SessionId = session.SessionId
            }
        );
        if( rows == 0 ) {
            throw new Exception( "No session found to indicate a visit." );
        }
    }
}
