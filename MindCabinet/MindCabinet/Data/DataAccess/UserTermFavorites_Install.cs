using Dapper;
using Microsoft.Data.SqlClient;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserTermFavorite;
using MindCabinet.Shared.Utility;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_UserTermFavorites : IServerDataAccess {
    public const string TableName = "UserTermFavorites";
    public const string TableColumn_SimpleUserId = "SimpleUserId";
    public const string TableColumn_FavTermId = "FavTermId";
    public const string TableColumn_Favor = "Favor";

    public async Task<bool> Install_Async( IDbConnection dbConnection ) {
        await dbConnection.ExecuteAsync( $@"
            CREATE TABLE {TableName} (
                {TableColumn_SimpleUserId} BIGINT NOT NULL,
                {TableColumn_FavTermId} BIGINT NOT NULL,
                {TableColumn_Favor} INT NOT NULL,
                 PRIMARY KEY ({TableColumn_SimpleUserId}, {TableColumn_FavTermId}),
                 CONSTRAINT FK_{TableName}_{TableColumn_SimpleUserId} FOREIGN KEY ({TableColumn_SimpleUserId})
                    REFERENCES {ServerDataAccess_SimpleUsers.TableName}({ServerDataAccess_SimpleUsers.TableColumn_Id}),
                 CONSTRAINT FK_{TableName}_{TableColumn_FavTermId} FOREIGN KEY ({TableColumn_FavTermId})
                    REFERENCES {ServerDataAccess_Terms.TableName}({ServerDataAccess_Terms.TableColumn_Id}),
                 INDEX IDX_{TableColumn_SimpleUserId} ({TableColumn_SimpleUserId}),
                 INDEX IDX_{TableColumn_SimpleUserId}And{TableColumn_Favor} ({TableColumn_SimpleUserId}, {TableColumn_Favor})
            );"
        //    ON DELETE CASCADE
        //    ON UPDATE CASCADE
        );

        return true;
    }
}
