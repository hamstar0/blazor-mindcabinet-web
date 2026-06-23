using Dapper;
using Microsoft.Data.SqlClient;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsContext;
using MindCabinet.Shared.Utility;
using System.Data;
using MindCabinet.Services;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_PostsContexts {
    public const string TableName = "PostsContexts";
    public const string TableColumn_Id = "Id";
    public const string TableColumn_Owner = "Owner";
    public const string TableColumn_Name = "Name";
    public const string TableColumn_Description = "Description";



    public async Task<bool> Install_Async( IDbConnection dbConnection ) {
        await dbConnection.ExecuteAsync( $@"
            CREATE TABLE {TableName} (
                {TableColumn_Id} BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                {TableColumn_Owner} BIGINT NOT NULL,
                {TableColumn_Name} VARCHAR(256) NOT NULL,
                {TableColumn_Description} MEDIUMTEXT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
                 CONSTRAINT FK_{TableName}_{TableColumn_Owner} FOREIGN KEY ({TableColumn_Owner})
                    REFERENCES {ServerDataAccess_SimpleUsers.TableName}({ServerDataAccess_SimpleUsers.TableColumn_Id})
            );"
                //  CONSTRAINT FK_{TableName}_SimpleUserId FOREIGN KEY (SimpleUserId)
                //     REFERENCES {ServerDataAccess_SimpleUsers.TableName}(Id)
        );

        return true;
    }
}
