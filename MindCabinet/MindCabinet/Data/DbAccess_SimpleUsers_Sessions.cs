using Dapper;
using Konscious.Security.Cryptography;
using MindCabinet.Client.Services;
using MindCabinet.Shared.DataEntries;
using System.Data;
using System.Text;


namespace MindCabinet.Data;


public partial class ServerDbAccess {
    public async Task InstallSimpleUserSessions_Async( IDbConnection dbConnection ) {
        await dbConnection.ExecuteAsync( @"
            CREATE TABLE SimpleUserSesions (
                Id VARCHAR(36) NOT NULL PRIMARY KEY NONCLUSTERED,
                IpAddress VARCHAR(45) NOT NULL,
                SimpleUserId BIGINT NOT NULL,
                FirstVisit DATETIME2(2) NOT NULL,
                LatestVisit DATETIME2(2) NOT NULL
                Visits INT NOT NULL,
                CONSTRAINT FK_UserId FOREIGN KEY (SimpleUserId)
                    REFERENCES SimpleUsers(Id)
            );"
        //    ON DELETE CASCADE
        //    ON UPDATE CASCADE
        );
    }


    public async Task CreateSimpleUserSession_Async(
                IDbConnection dbCon,
                SimpleUserEntry user,
                ServerSessionData session ) {
        if( !session.IsLoaded ) {
            throw new Exception( "Session not loaded." );
        }

        var sessData = await dbCon.QuerySingleAsync<SimpleUserEntry.SessionDbData?>(
            "SELECT * FROM SimpleUserSession WHERE Id = @SessionId",
            new { SessionId = session.SessionId }
        );
        if( sessData is not null ) {
            throw new Exception( "Session already exists." );
        }

        DateTime now = DateTime.UtcNow;

        await dbCon.QuerySingleAsync(
            @"INSERT INTO SimpleUserSession
                (Id, IpAddress, SimpleUserId, FirstVisit, LatestVisit, Visits) 
                VALUES (@Id, @IpAddress, @SimpleUserId, @FirstVisit, @LatestVisit, @Visits)",
            new {
                Id = session.SessionId,
                IpAddress = session.IpAddress ?? "",
                SimpleUserId = user.Id,
                FirstVisit = now,
                LatestVisit = now,
                Visits = 1
            }
        );
    }


    public async Task VisitSimpleUserSession_Async(
                IDbConnection dbCon,
                ServerSessionData session ) {
        if( !session.IsLoaded ) {
            throw new Exception( "Session not loaded." );
        }

        int rows = await dbCon.ExecuteAsync(
            @"UPDATE SimpleUserSession
                SET Visits = Visits + 1, LatestVisit = @Now
                WHERE Id = @SessionId",
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
