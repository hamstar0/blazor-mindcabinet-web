using Dapper;
using Microsoft.Data.SqlClient;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserContext;
using MindCabinet.Shared.Utility;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_UserAppData : IServerDataAccess {
    public const string TableName = "UserAppData";



    public async Task<bool> Install_Async(
                IDbConnection dbConnection,
                long defaultUserId,
                UserContextObject sampleContext ) {
        await dbConnection.ExecuteAsync( $@"
            CREATE TABLE {TableName} (
                SimpleUserId BIGINT NOT NULL PRIMARY KEY,
                ContextId BIGINT NOT NULL,
                Description MEDIUMTEXT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
                CONSTRAINT FK_{TableName}_SimpleUserId FOREIGN KEY (SimpleUserId)
                    REFERENCES {ServerDataAccess_SimpleUsers.TableName}(Id)
                CONSTRAINT FK_{TableName}_ContextId FOREIGN KEY (ContextId)
                    REFERENCES {ServerDataAccess_UserContexts.TableName}(Id)
            );"
        );

        return await this.InstallSamples_Async( dbConnection, defaultUserId, sampleContext );
    }
    

    public async Task<UserAppDataObject?> GetById_Async( IDbConnection dbCon,
                ServerDataAccess_Terms termsData,
                ServerDataAccess_TermSets termSetsData,
                ServerDataAccess_UserContexts userContextsData,
                long userId ) {
        UserAppDataObject.DatabaseEntry? usrAppDataRaw = await dbCon.QuerySingleAsync<UserAppDataObject.DatabaseEntry?>(
            $"SELECT * FROM {TableName} WHERE SimpleUserId = @SimpleUserId",
            new { SimpleUserId = userId }
        );

        if( usrAppDataRaw is null ) {
            return null;
        }

        return await usrAppDataRaw.CreateUserAppDataObject_Async(
            async (usrCtxId) => new IdDataObject<UserContextObject> {
                Id = usrCtxId,
                Data = await userContextsData.GetById_Async( dbCon, termsData, termSetsData, usrCtxId )
            }
        );
    }


    public async Task<UserAppDataObject> Create_Async(
                IDbConnection dbCon,
                long simpleUserId,
                long userContextId ) {
        long _ = await dbCon.ExecuteScalarAsync<long>(
            $@"INSERT INTO {TableName} (SimpleUserId, ContextId) 
                VALUES (@SimpleUserId, @ContextId);
            SELECT LAST_INSERT_ID();",
            new {
                SimpleUserId = simpleUserId,
                ContextId = userContextId
            }
        );

        return new UserAppDataObject(
            simpleUserId,
            new IdDataObject<UserContextObject> { Id = userContextId }
        );
    }
}
