using Dapper;
using MindCabinet.DataObjects;
using MindCabinet.Services;
using MindCabinet.Shared.DataObjects;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_SimpleUserSessions : IServerDataAccess {
    public const string TableName = "SimpleUserSessions";
    public const string TableColumn_Id = "Id";
    public const string TableColumn_LatestIpAddress = "LatestIpAddress";
    public const string TableColumn_SimpleUserId = "SimpleUserId";
    public const string TableColumn_FirstVisit = "FirstVisit";
    public const string TableColumn_LatestVisit = "LatestVisit";
    public const string TableColumn_Visits = "Visits";

    public async Task<bool> Install_Async( IDbConnection dbConnection ) {
        await dbConnection.ExecuteAsync( $@"
            CREATE TABLE {TableName} (
                {TableColumn_Id} VARCHAR(36) NOT NULL PRIMARY KEY,
                {TableColumn_LatestIpAddress} VARCHAR(45) NOT NULL,
                {TableColumn_SimpleUserId} BIGINT NOT NULL,
                {TableColumn_FirstVisit} DATETIME(2) NOT NULL,
                {TableColumn_LatestVisit} DATETIME(2) NOT NULL,
                {TableColumn_Visits} INT NOT NULL,
                 CONSTRAINT FK_{TableName}_{TableColumn_SimpleUserId} FOREIGN KEY ({TableColumn_SimpleUserId})
                    REFERENCES {ServerDataAccess_SimpleUsers.TableName}({ServerDataAccess_SimpleUsers.TableColumn_Id})
            );"
        //       PRIMARY KEY (Id),
        //    ON DELETE CASCADE
        //    ON UPDATE CASCADE
        );

        return true;
    }
}
