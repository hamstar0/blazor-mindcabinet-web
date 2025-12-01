using Dapper;
using Microsoft.Data.SqlClient;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.Utility;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_UserTermsHistory {
    public const string TableName = "UserTermsHistory";

    public async Task<bool> Install_Async( IDbConnection dbConnection ) {
        await dbConnection.ExecuteAsync( @"
            CREATE TABLE "+TableName+@" (
                SimpleUserId BIGINT NOT NULL,
                TermId BIGINT NOT NULL,
                Created DATETIME(2) NOT NULL,
                CONSTRAINT FK_SimpleUserId FOREIGN KEY (SimpleUserId)
                    REFERENCES SimpleUsers(Id),
                CONSTRAINT FK_TermId FOREIGN KEY (TermId)
                    REFERENCES Terms(Id),
                INDEX IDX_User (SimpleUserId),
                INDEX IDX_UserCreated (SimpleUserId, Created)
            );"
        //    ON DELETE CASCADE
        //    ON UPDATE CASCADE
        );

        return true;
    }
    

    public async Task<IEnumerable<ClientDataAccess_UserTermsHistory.GetByUserId_Return>> GetByUserId_Async(
                IDbConnection dbCon,
                ClientDataAccess_UserTermsHistory.GetByUserId_Params parameters ) {
        string sql = $"SELECT TermId, Created FROM {TableName} WHERE SimpleUserId = @UserId;";
        var sqlParams = new Dictionary<string, object> { { "@UserId", parameters.UserId } };

        return await dbCon.QueryAsync<ClientDataAccess_UserTermsHistory.GetByUserId_Return>( sql, new DynamicParameters(sqlParams) );
	}


    public async Task AddTerm_Async(
                IDbConnection dbCon,
                long simpleUserId,
                ClientDataAccess_UserTermsHistory.Add_Params parameters ) {
        await dbCon.ExecuteAsync(
            @"INSERT INTO "+TableName+@" (SimpleUserId, TermId, Created) 
                VALUES (@SimpleUserId, @TermId, @Created);",
            new {
                SimpleUserId = simpleUserId,
                TermId = parameters.TermId,
                Created = DateTime.UtcNow,
            }
        );
    }
}
