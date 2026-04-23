using Dapper;
using Microsoft.Data.SqlClient;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsContext;
using MindCabinet.Shared.Utility;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_UserAppData : IServerDataAccess {
    public const string TableName = "UserAppData";



    public async Task<bool> Install_Async( IDbConnection dbConnection ) {
        await dbConnection.ExecuteAsync( $@"
            CREATE TABLE {TableName} (
                SimpleUserId BIGINT NOT NULL PRIMARY KEY,
                PostsContextId BIGINT NOT NULL,
                 CONSTRAINT FK_{TableName}_SimpleUserId FOREIGN KEY (SimpleUserId)
                    REFERENCES {ServerDataAccess_SimpleUsers.TableName}(Id),
                 CONSTRAINT FK_{TableName}_PostsContextId FOREIGN KEY (PostsContextId)
                    REFERENCES {ServerDataAccess_PostsContexts.TableName}(Id)
            );"
        );

        return true;
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
                PostsContextId userDefaultPostsContextId ) {
        if( simpleUserId == 0 ) {
            throw new ArgumentException( "SimpleUserId is not valid (must be non-zero)." );
        }
        if( userDefaultPostsContextId == 0 ) {
            throw new ArgumentException( "PostsContextId is not valid (must be non-zero)." );
        }

        try {
            int rows = await dbCon.ExecuteAsync(
                $@"INSERT INTO {TableName} (SimpleUserId, PostsContextId) 
                    VALUES (@SimpleUserId, @PostsContextId);",
                new {
                    SimpleUserId = (long)simpleUserId,
                    PostsContextId = (long)userDefaultPostsContextId
                }
            );
        } catch( Exception e ) { //when ( ex.Number == 1062 ) {
            throw new InvalidOperationException(
                $@"Record could not be created (SimpleUserId: {simpleUserId}, PostsContextId: {userDefaultPostsContextId})",
                e
            );
        }

        return UserAppDataObject.CreateRaw(
            simpleUserId: simpleUserId,
            postsContextId: userDefaultPostsContextId
        );
    }

    public async Task Update_Async(
                IDbConnection dbCon,
                SimpleUserId simpleUserId,
                PostsContextId postsContextId ) {
        if( simpleUserId == 0 ) {
            throw new ArgumentException( "SimpleUserId is not valid (must be non-zero)." );
        }
        if( postsContextId == 0 ) {
            throw new ArgumentException( "PostsContextId is not valid (must be non-zero)." );
        }

        try {
            await dbCon.ExecuteAsync(
                $@"UPDATE {TableName}
                    SET PostsContextId = @PostsContextId
                    WHERE SimpleUserId = @SimpleUserId;",
                new {
                    PostsContextId = postsContextId,
                    SimpleUserId = simpleUserId
                }
            );
        } catch( Exception e ) { //when ( ex.Number == 1062 ) {
            throw new InvalidOperationException( $"Record could not be updated (SimpleUserId: {simpleUserId}, PostsContextId: {postsContextId})", e );
        }
    }
}
