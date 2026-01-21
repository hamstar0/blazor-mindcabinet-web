using Dapper;
using Microsoft.Data.SqlClient;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.Utility;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_UserTermsHistory : IServerDataAccess {
    public const int HistoryMaxEntries = 100;


    public const string TableName = "UserTermsHistory";

    public async Task<bool> Install_Async( IDbConnection dbConnection ) {
        await dbConnection.ExecuteAsync( $@"
            CREATE TABLE {TableName} (
                SimpleUserId BIGINT NOT NULL,
                TermId BIGINT NOT NULL,
                Created DATETIME(2) NOT NULL,
                CONSTRAINT FK_UserTermsHistory_SimpleUserId FOREIGN KEY (SimpleUserId)
                    REFERENCES {ServerDataAccess_SimpleUsers.TableName}(Id),
                CONSTRAINT FK_UserTermsHistory_TermId FOREIGN KEY (TermId)
                    REFERENCES {ServerDataAccess_Terms.TableName}(Id),
                INDEX IDX_User (SimpleUserId),
                INDEX IDX_UserCreated (SimpleUserId, Created)
            );"
        //    ON DELETE CASCADE
        //    ON UPDATE CASCADE
        );

        return true;
    }
    

    public async Task<IEnumerable<ClientDataAccess_UserTermsHistory.GetTermIdsForCurrentUser_Return>> GetByUserId_Async(
                IDbConnection dbCon,
                long simpleUserId,
                ClientDataAccess_UserTermsHistory.GetTermIdsForCurrentUser_Params parameters ) {
        string sql = $"SELECT TermId, Created FROM {TableName} WHERE SimpleUserId = @UserId;";
        var sqlParams = new Dictionary<string, object> { { "@UserId", simpleUserId } };

        return await dbCon.QueryAsync<ClientDataAccess_UserTermsHistory.GetTermIdsForCurrentUser_Return>( sql, new DynamicParameters(sqlParams) );
	}


    public async Task AddTerm_Async(
                IDbConnection dbCon,
                long simpleUserId,
                ClientDataAccess_UserTermsHistory.AddTermsForCurrentUser_Params parameters ) {
        await dbCon.ExecuteAsync(
            $@"INSERT INTO {TableName} (SimpleUserId, TermId, Created) 
                VALUES (@SimpleUserId, @TermId, @Created);",
            new {
                SimpleUserId = simpleUserId,
                TermId = parameters.TermId,
                Created = DateTime.UtcNow,
            }
        );

        // int count = await dbCon.ExecuteScalarAsync<int>(
        //     @"SELECT COUNT(*) FROM "+TableName+@" 
        //         WHERE SimpleUserId = @SimpleUserId;",
        //     new {
        //         SimpleUserId = simpleUserId,
        //     }
        // );
        // if( count > ServerDataAccess_UserTermsHistory.HistoryMaxEntries ) {

        await dbCon.ExecuteAsync(
            $@"DELETE FROM {TableName} AS Trimmed
                WHERE Trimmed.SimpleUserId = @SimpleUserId
                AND Trimmed.Created NOT IN (
                    SELECT Kept.Created FROM {TableName} AS Kept
                    WHERE Kept.SimpleUserId = @SimpleUserId
                    ORDER BY Kept.Created ASC
                    LIMIT @AllowedCount
                );",
            new {
                SimpleUserId = simpleUserId,
                AllowedCount = ServerDataAccess_UserTermsHistory.HistoryMaxEntries,
            }
        );
    }
}
