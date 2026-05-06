using Dapper;
using Microsoft.Data.SqlClient;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.UserTermHistory;
using MindCabinet.Shared.Utility;
using System.Data;

namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_UserTermsHistory : IServerDataAccess {
    public const string TableName = "UserTermsHistory";
    public const string TableColumn_SimpleUserId = "SimpleUserId";
    public const string TableColumn_TermId = "TermId";
    public const string TableColumn_Created = "Created";

    public async Task<bool> Install_Async( IDbConnection dbConnection ) {
        await dbConnection.ExecuteAsync( $@"
            CREATE TABLE {TableName} (
                {TableColumn_SimpleUserId} BIGINT NOT NULL,
                {TableColumn_TermId} BIGINT NOT NULL,
                {TableColumn_Created} DATETIME(2) NOT NULL,
                 CONSTRAINT FK_{TableName}_{TableColumn_SimpleUserId} FOREIGN KEY ({TableColumn_SimpleUserId})
                    REFERENCES {ServerDataAccess_SimpleUsers.TableName}({ServerDataAccess_SimpleUsers.TableColumn_Id}),
                 CONSTRAINT FK_{TableName}_{TableColumn_TermId} FOREIGN KEY ({TableColumn_TermId})
                    REFERENCES {ServerDataAccess_Terms.TableName}({ServerDataAccess_Terms.TableColumn_Id}),
                 INDEX IDX_{TableColumn_SimpleUserId} ({TableColumn_SimpleUserId}),
                 INDEX IDX_{TableColumn_Created} ({TableColumn_Created})
            );"
        //    ON DELETE CASCADE
        //    ON UPDATE CASCADE
        );

        return true;
    }
}
