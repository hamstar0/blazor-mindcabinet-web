using Dapper;
using Microsoft.Data.SqlClient;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsContext;
using MindCabinet.Shared.Utility;
using System.Data;
using MindCabinet.DataObjects;
using MindCabinet.Services;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_ServerData : IServerDataAccess {
    public const string TableName = "ServerData";
    public const string TableColumn_UsersConceptTermId = "UsersConceptTermId";



    public async Task<bool> Install_Async( IDbConnection dbConnection ) {
        await dbConnection.ExecuteAsync( $@"
            CREATE TABLE {TableName} (
                {TableColumn_UsersConceptTermId} BIGINT NOT NULL,
                 CONSTRAINT FK_{TableName}_{TableColumn_UsersConceptTermId} FOREIGN KEY ({TableColumn_UsersConceptTermId})
                    REFERENCES {ServerDataAccess_Terms.TableName}({ServerDataAccess_Terms.TableColumn_Id})
            );"
        );

        return true;
    }

    public async Task<bool> Install_After_Async(
                IDbConnection dbConnection,
                TermId usersConceptTermId ) {
        await this.Create_Async( dbConnection, usersConceptTermId );

        return true;
    }
}
