using Dapper;
using Microsoft.Data.SqlClient;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsQuery;
using MindCabinet.Shared.Utility;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_UserAppData(
                ILogger<ServerDataAccess_UserAppData> logger,
                StaticServerSettings serverSettings
            ) : IServerDataAccess {
    private static readonly SimpleCache<SimpleUserId, UserAppDataObject.Raw?> Cache_BySimpleUserId = new( refreshExpiryOnGet: true );



    private readonly ILogger<ServerDataAccess_UserAppData> Logger = logger;

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
                PostsQueryId userDefaultPostsQueryId,
                TermId userDefaultTermId ) {
        if( simpleUserId == 0 ) {
            throw new ArgumentException( "SimpleUserId is not valid (must be non-zero)." );
        }
        if( userDefaultPostsQueryId == 0 ) {
            throw new ArgumentException( "PostsQueryId is not valid (must be non-zero)." );
        }

        try {
            int rows = await dbCon.ExecuteAsync(
                $@"INSERT INTO {TableName} ({TableColumn_SimpleUserId}, {TableColumn_PostsQueryId}, {TableColumn_UserDefaultTermId}) 
                    VALUES (@SimpleUserId, @PostsQueryId, @userDefaultTermId);",
                new {
                    SimpleUserId = (long)simpleUserId,
                    PostsQueryId = (long)userDefaultPostsQueryId,
                    userDefaultTermId = (long)userDefaultTermId
                }
            );
        } catch( Exception e ) { //when ( ex.Number == 1062 ) {
            throw new InvalidOperationException(
                message: $@"Record could not be created (SimpleUserId: {simpleUserId}, PostsQueryId: {userDefaultPostsQueryId})",
                innerException: e
            );
        }

        UserAppDataObject.Raw raw = UserAppDataObject.CreateRaw(
            simpleUserId: simpleUserId,
            currentPostsQueryId: userDefaultPostsQueryId,
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
                PostsQueryId postsQueryId,
                TermId userDefaultTermId ) {    //todo
        if( simpleUserId == 0 ) {
            throw new ArgumentException( "SimpleUserId is not valid (must be non-zero)." );
        }
        if( postsQueryId == 0 ) {
            throw new ArgumentException( "PostsQueryId is not valid (must be non-zero)." );
        }
        if( userDefaultTermId == 0 ) {
            throw new ArgumentException( "TermId is not valid (must be non-zero)." );
        }

        //

        try {
            await dbCon.ExecuteAsync(
                $@"UPDATE {TableName}
                    SET {TableColumn_PostsQueryId} = @PostsQueryId,
                        {TableColumn_UserDefaultTermId} = @UserDefaultTermId
                    WHERE {TableColumn_SimpleUserId} = @SimpleUserId;",
                new {
                    PostsQueryId = postsQueryId,
                    UserDefaultTermId = userDefaultTermId,
                    SimpleUserId = simpleUserId
                }
            );
        } catch( Exception e ) { //when ( ex.Number == 1062 ) {
            throw new InvalidOperationException( $"Record could not be updated (SimpleUserId: {simpleUserId}, PostsQueryId: {postsQueryId})", e );
        }
    }
}
