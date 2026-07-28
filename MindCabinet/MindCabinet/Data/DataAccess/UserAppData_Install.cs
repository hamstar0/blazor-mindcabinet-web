using Dapper;
using Microsoft.Data.SqlClient;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsQuery;
using MindCabinet.Shared.Utility;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_UserAppData : IServerDataAccess {
    public const string TableName = "UserAppData";
    public const string TableColumn_SimpleUserId = "SimpleUserId";
    public const string TableColumn_PostsQueryId = "CurrentPostsQueryId";
    public const string TableColumn_UserDefaultTermId = "UserDefaultTermId";



    public async Task<bool> Install_Async( IDbConnection dbConnection ) {
        await dbConnection.ExecuteAsync( $@"
            CREATE TABLE {TableName} (
                {TableColumn_SimpleUserId} BIGINT NOT NULL PRIMARY KEY,
                {TableColumn_PostsQueryId} BIGINT NOT NULL,
                {TableColumn_UserDefaultTermId} BIGINT NOT NULL,
                 CONSTRAINT FK_{TableName}_{TableColumn_SimpleUserId} FOREIGN KEY ({TableColumn_SimpleUserId})
                    REFERENCES {ServerDataAccess_SimpleUsers.TableName}({ServerDataAccess_SimpleUsers.TableColumn_Id}),
                 CONSTRAINT FK_{TableName}_{TableColumn_PostsQueryId} FOREIGN KEY ({TableColumn_PostsQueryId})
                    REFERENCES {ServerDataAccess_PostsQuery.TableName}({ServerDataAccess_PostsQuery.TableColumn_Id}),
                 CONSTRAINT FK_{TableName}_{TableColumn_UserDefaultTermId} FOREIGN KEY ({TableColumn_UserDefaultTermId})
                    REFERENCES {ServerDataAccess_Terms.TableName}({ServerDataAccess_Terms.TableColumn_Id})
            );"
        );

        return true;
    }
}
