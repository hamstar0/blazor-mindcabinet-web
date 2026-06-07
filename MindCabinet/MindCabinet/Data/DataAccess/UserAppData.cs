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


public partial class ServerDataAccess_UserAppData(
                StaticServerSettings serverSettings
            ) : IServerDataAccess {
    private static readonly SimpleCache<SimpleUserId, UserAppDataObject.Raw?> Cache_BySimpleUserId = new( refreshOnGet: true );



    private readonly StaticServerSettings ServerSettings = serverSettings;



    public async Task<UserAppDataObject.Raw?> GetById_Async(
                IDbConnection dbCon,
                SimpleUserId id ) {
        if( id == 0 ) {
            throw new ArgumentException( "SimpleUserId is not valid (must be non-zero)." );
        }

        //

        if( ServerDataAccess_UserAppData.Cache_BySimpleUserId.TryGet(id, out var cached) ) {
            return cached;
        }

        //

        UserAppDataObject.Raw? usrAppDataRaw = await dbCon.QuerySingleOrDefaultAsync<UserAppDataObject.Raw>(
            $"SELECT * FROM {TableName} WHERE SimpleUserId = @SimpleUserId",
            new { SimpleUserId = (long)id }
        );

        //

        ServerDataAccess_UserAppData.Cache_BySimpleUserId.Set(
            key: id,
            value: usrAppDataRaw,
            expiry: this.ServerSettings.CacheExpirationDuration
        );

        //

        return usrAppDataRaw;
    }


    public async Task<UserAppDataObject.Raw> Create_Async(
                IDbConnection dbCon,
                SimpleUserId simpleUserId,
                PostsContextId userDefaultPostsContextId,
                TermId userDefaultTermId ) {
        if( simpleUserId == 0 ) {
            throw new ArgumentException( "SimpleUserId is not valid (must be non-zero)." );
        }
        if( userDefaultPostsContextId == 0 ) {
            throw new ArgumentException( "PostsContextId is not valid (must be non-zero)." );
        }

        try {
            int rows = await dbCon.ExecuteAsync(
                $@"INSERT INTO {TableName} ({TableColumn_SimpleUserId}, {TableColumn_PostsContextId}, {TableColumn_UserDefaultTermId}) 
                    VALUES (@SimpleUserId, @PostsContextId, @userDefaultTermId);",
                new {
                    SimpleUserId = (long)simpleUserId,
                    PostsContextId = (long)userDefaultPostsContextId,
                    userDefaultTermId = (long)userDefaultTermId
                }
            );
        } catch( Exception e ) { //when ( ex.Number == 1062 ) {
            throw new InvalidOperationException(
                message: $@"Record could not be created (SimpleUserId: {simpleUserId}, PostsContextId: {userDefaultPostsContextId})",
                innerException: e
            );
        }

        UserAppDataObject.Raw raw = UserAppDataObject.CreateRaw(
            simpleUserId: simpleUserId,
            currentPostsContextId: userDefaultPostsContextId,
            userDefaultTermId: userDefaultTermId
        );

        //

        ServerDataAccess_UserAppData.Cache_BySimpleUserId.Set(
            key: simpleUserId,
            value: raw,
            expiry: this.ServerSettings.CacheExpirationDuration
        );

        //

        return raw;
    }

    public async Task Update_Async(
                IDbConnection dbCon,
                SimpleUserId simpleUserId,
                PostsContextId postsContextId,
                TermId userDefaultTermId ) {
        if( simpleUserId == 0 ) {
            throw new ArgumentException( "SimpleUserId is not valid (must be non-zero)." );
        }
        if( postsContextId == 0 ) {
            throw new ArgumentException( "PostsContextId is not valid (must be non-zero)." );
        }
        if( userDefaultTermId == 0 ) {
            throw new ArgumentException( "TermId is not valid (must be non-zero)." );
        }

        try {
            await dbCon.ExecuteAsync(
                $@"UPDATE {TableName}
                    SET {TableColumn_PostsContextId} = @PostsContextId,
                        {TableColumn_UserDefaultTermId} = @UserDefaultTermId
                    WHERE {TableColumn_SimpleUserId} = @SimpleUserId;",
                new {
                    PostsContextId = postsContextId,
                    UserDefaultTermId = userDefaultTermId,
                    SimpleUserId = simpleUserId
                }
            );
        } catch( Exception e ) { //when ( ex.Number == 1062 ) {
            throw new InvalidOperationException( $"Record could not be updated (SimpleUserId: {simpleUserId}, PostsContextId: {postsContextId})", e );
        }
    }
}
