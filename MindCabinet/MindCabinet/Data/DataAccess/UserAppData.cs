using Dapper;
using Microsoft.Data.SqlClient;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserPostsContext;
using MindCabinet.Shared.Utility;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_UserAppData : IServerDataAccess {
    public const string TableName = "UserAppData";



    public async Task<bool> Install_Async(
                IDbConnection dbConnection,
                SimpleUserId defaultUserId,
                UserPostsContextObject.Raw sampleContext ) {
        await dbConnection.ExecuteAsync( $@"
            CREATE TABLE {TableName} (
                SimpleUserId BIGINT NOT NULL PRIMARY KEY,
                UserPostsContextId BIGINT NOT NULL,
                 CONSTRAINT FK_{TableName}_SimpleUserId FOREIGN KEY (SimpleUserId)
                    REFERENCES {ServerDataAccess_SimpleUsers.TableName}(Id),
                 CONSTRAINT FK_{TableName}_UserPostsContextId FOREIGN KEY (UserPostsContextId)
                    REFERENCES {ServerDataAccess_UserPostsContexts.TableName}(Id)
            );"
        );

        return await this.InstallSamples_Async( dbConnection, defaultUserId, sampleContext );
    }
    

    public async Task<UserAppDataObject.Raw?> GetById_Async(
                IDbConnection dbCon,
                SimpleUserId id ) {
        UserAppDataObject.Raw? usrAppDataRaw = await dbCon.QuerySingleOrDefaultAsync<UserAppDataObject.Raw>(
            $"SELECT * FROM {TableName} WHERE SimpleUserId = @SimpleUserId",
            new { SimpleUserId = (long)id }
        );

        return usrAppDataRaw;
    }


    public async Task<UserAppDataObject.Raw> Create_Async(
                IDbConnection dbCon,
                SimpleUserId simpleUserId,
                UserPostsContextId userPostsContextId ) {
        long _ = await dbCon.ExecuteScalarAsync<long>(
            $@"INSERT INTO {TableName} (SimpleUserId, UserPostsContextId) 
                VALUES (@SimpleUserId, @UserPostsContextId);
            SELECT LAST_INSERT_ID();",
            new {
                SimpleUserId = simpleUserId,
                UserPostsContextId = userPostsContextId
            }
        );

        return new UserAppDataObject.Raw {
            SimpleUserId = simpleUserId,
            UserPostsContextId = userPostsContextId
        };
    }
}
