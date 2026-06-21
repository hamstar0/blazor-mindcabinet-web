using Dapper;
using Microsoft.Data.SqlClient;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.UserTermHistory;
using MindCabinet.Shared.Utility;
using System.Data;

namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_UserTermsHistory(
                StaticServerSettings serverSettings
            ) : IServerDataAccess {
    public const int HistoryMaxEntries = 100;


    private static readonly SimpleCache<SimpleUserId, IEnumerable<UserTermHistoryObject.Raw>> Cache_BySimpleUserId = new( refreshExpiryOnGet: true );



    private readonly StaticServerSettings ServerSettings = serverSettings;



    public async Task<IEnumerable<UserTermHistoryObject.Raw>> GetByUserId_Async(
                IDbConnection dbCon,
                SimpleUserId simpleUserId ) {
        if( simpleUserId == 0 ) {
            throw new ArgumentException( "SimpleUserId is not valid (must be non-zero)." );
        }

        //

        if( ServerDataAccess_UserTermsHistory.Cache_BySimpleUserId.TryGet(simpleUserId, out var cached) ) {
            return cached;
        }

        //

        string sql = $"SELECT * FROM {TableName} WHERE {TableColumn_SimpleUserId} = @SimpleUserId;";
        var sqlParams = new Dictionary<string, object> { { "@SimpleUserId", (long)simpleUserId } };

        var termsHistory = await dbCon.QueryAsync<UserTermHistoryObject.Raw>(
            sql,
            new DynamicParameters(sqlParams)
        );

        //

        ServerDataAccess_UserTermsHistory.Cache_BySimpleUserId.Set(
            key: simpleUserId,
            value: termsHistory,
            expiry: this.ServerSettings.CacheExpirationDuration
        );

        //

        return termsHistory;
	}


    public async Task AddTerm_Async(
                IDbConnection dbCon,
                SimpleUserId simpleUserId,
                ClientDataAccess_UserTermsHistory.IAPI.AddHistTermsForCurrentUser_Params parameters ) {
        if( simpleUserId == 0 ) {
            throw new ArgumentException( "SimpleUserId is not valid (must be non-zero)." );
        }
        if( parameters.TermId == 0 ) {
            throw new ArgumentException( "TermId is not valid (must be non-zero)." );
        }

        await dbCon.ExecuteAsync(
            $@"INSERT INTO {TableName} ({TableColumn_SimpleUserId}, {TableColumn_TermId}, {TableColumn_Created}) 
                VALUES (@SimpleUserId, @TermId, @Created);",
            new {
                SimpleUserId = (long)simpleUserId,
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

        IEnumerable<long> idsToKeep = await dbCon.QueryAsync<long>(
            $@"SELECT {TableColumn_TermId} FROM {TableName}
                WHERE {TableColumn_SimpleUserId} = @SimpleUserId
                ORDER BY {TableColumn_Created} DESC
                LIMIT @AllowedCount;",
            new {
                SimpleUserId = (long)simpleUserId,
                AllowedCount = ServerDataAccess_UserTermsHistory.HistoryMaxEntries
            }
        );

        await dbCon.ExecuteAsync(
            $@"DELETE FROM {TableName}
                WHERE {TableColumn_SimpleUserId} = @SimpleUserId
                AND {TableColumn_TermId} NOT IN @IdsToKeep;",
            new {
                SimpleUserId = (long)simpleUserId,
                IdsToKeep = idsToKeep
            }
        );

        //

        ServerDataAccess_UserTermsHistory.Cache_BySimpleUserId.Remove( simpleUserId );
    }
}
