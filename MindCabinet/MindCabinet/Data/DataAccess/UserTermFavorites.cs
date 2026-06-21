using Dapper;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserTermFavorite;
using MindCabinet.Shared.Utility;
using System.Data;
using System.Text;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_UserTermFavorites(
                StaticServerSettings serverSettings
            ) : IServerDataAccess {
    private static readonly SimpleCache<SimpleUserId, IEnumerable<UserTermFavoriteObject.Raw>> Cache_BySimpleUserId = new( refreshExpiryOnGet: true );



    private readonly StaticServerSettings ServerSettings = serverSettings;



    public async Task<IEnumerable<UserTermFavoriteObject.Raw>> GetFavTermEntriesBySimpleUserId_Async(
                IDbConnection dbCon,
                SimpleUserId simpleUserId ) {
        if( simpleUserId == 0 ) {
            throw new ArgumentException( "SimpleUserId is not valid (must be non-zero)." );
        }

        //

        if( ServerDataAccess_UserTermFavorites.Cache_BySimpleUserId.TryGet(simpleUserId, out var cached) ) {
            return cached;
        }

        //

        string sql = $"SELECT * FROM {TableName} WHERE {TableColumn_SimpleUserId} = @UserId;";
        var sqlParams = new Dictionary<string, object> { { "@UserId", (long)simpleUserId } };

        var favTermEntries = await dbCon.QueryAsync<UserTermFavoriteObject.Raw>(
            sql,
            new DynamicParameters(sqlParams)
        );

        //

        ServerDataAccess_UserTermFavorites.Cache_BySimpleUserId.Set(
            key: simpleUserId,
            value: favTermEntries,
            expiry: this.ServerSettings.CacheExpirationDuration
        );

        //

        return favTermEntries;
	}


    public async Task AddFavTermEntries_Async(
                IDbConnection dbCon,
                SimpleUserId simpleUserId,
                TermId[] favTermIds ) {
        if( simpleUserId == 0 ) {
            throw new ArgumentException( "SimpleUserId is not valid (must be non-zero)." );
        }
        if( favTermIds.Any(id => id == 0) ) {
            throw new ArgumentException( "FavTermIds contains invalid values (must be non-zero)." );
        }
        if( favTermIds.Length == 0 ) {
            return;
        }

        var sqlBuilder = new StringBuilder(
            @$"INSERT INTO {TableName}
                ({TableColumn_SimpleUserId}, {TableColumn_FavTermId}, {TableColumn_Favor})
            VALUES "
        );
        var sqlParams = new DynamicParameters();

        for( int i=0; i<favTermIds.Length; i++ ) {
            if( i > 0 ) {
                sqlBuilder.Append( ", " );
            }

            string simpleUserParamName = $"@SimpleUserId{i}";
            string favTermParamName = $"@FavTermId{i}";
            sqlBuilder.Append( $"({simpleUserParamName}, {favTermParamName}, 0)" );

            sqlParams.Add( simpleUserParamName, (long)simpleUserId );
            sqlParams.Add( favTermParamName, (long)favTermIds[i] );
        }

        await dbCon.ExecuteAsync( sqlBuilder.ToString(), sqlParams );

        //

        ServerDataAccess_UserTermFavorites.Cache_BySimpleUserId.Remove( simpleUserId );
    }


    public async Task RemoveFavTermEntries_Async(
                IDbConnection dbCon,
                SimpleUserId simpleUserId,
                ClientDataAccess_UserTermFavorites.IAPI.RemoveTermsForCurrentUser_Params parameters ) {
        if( simpleUserId == 0 ) {
            throw new ArgumentException( "SimpleUserId is not valid (must be non-zero)." );
        }
        if( parameters.TermIds.Any(id => id == 0) ) {
            throw new ArgumentException( "FavTermIds contains invalid values (must be non-zero)." );
        }

        await dbCon.ExecuteAsync(
            $@"DELETE FROM {TableName}
                WHERE {TableColumn_SimpleUserId} = @SimpleUserId
                    AND {TableColumn_FavTermId} IN @TermIds",
            new {
                SimpleUserId = (long)simpleUserId,
                TermIds = parameters.TermIds
            }
        );

        //

        ServerDataAccess_UserTermFavorites.Cache_BySimpleUserId.Remove( simpleUserId );
    }
}
