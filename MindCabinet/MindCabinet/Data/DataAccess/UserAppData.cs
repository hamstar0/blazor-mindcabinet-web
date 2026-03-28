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
                UserPostsContextId sampleContextId ) {
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

        return await this.InstallSamples_Async( dbConnection, defaultUserId, sampleContextId );
    }
    


    public async Task<UserAppDataObject.Raw?> GetById_Async(
                IDbConnection dbCon,
                SimpleUserId id ) {
        if( id == 0 ) {
            throw new ArgumentException( "SimpleUserId is not valid (must be non-zero)." );
        }

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
        if( simpleUserId == 0 ) {
            throw new ArgumentException( "SimpleUserId is not valid (must be non-zero)." );
        }
        if( userPostsContextId == 0 ) {
            throw new ArgumentException( "UserPostsContextId is not valid (must be non-zero)." );
        }

        try {
            long _ = await dbCon.ExecuteScalarAsync<long>(
                $@"INSERT INTO {TableName} (SimpleUserId, UserPostsContextId) 
                    VALUES (@SimpleUserId, @UserPostsContextId);
                SELECT LAST_INSERT_ID();",
                new {
                    SimpleUserId = (long)simpleUserId,
                    UserPostsContextId = (long)userPostsContextId
                }
            );
        } catch( Exception e ) { //when ( ex.Number == 1062 ) {
            throw new InvalidOperationException( $"Record could not be created (SimpleUserId: {simpleUserId}, UserPostsContextId: {userPostsContextId})", e );
        }

        return UserAppDataObject.CreateRaw(
            simpleUserId: simpleUserId,
            userPostsContextId: userPostsContextId
        );
    }
}
