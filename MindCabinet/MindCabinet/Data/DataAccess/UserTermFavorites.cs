using Dapper;
using Microsoft.Data.SqlClient;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserTermFavorite;
using MindCabinet.Shared.Utility;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_UserTermFavorites : IServerDataAccess {
    public async Task<IEnumerable<UserTermFavoriteObject.Raw>> GetFavTermEntries_Async(
                IDbConnection dbCon,
                SimpleUserId simpleUserId,
                ClientDataAccess_UserTermFavorites.IAPI.GetFavTermsForCurrentUser_Params parameters ) {
        if( simpleUserId == 0 ) {
            throw new ArgumentException( "SimpleUserId is not valid (must be non-zero)." );
        }

        string sql = $"SELECT * FROM {TableName} WHERE {TableColumn_SimpleUserId} = @UserId;";
        var sqlParams = new Dictionary<string, object> { { "@UserId", (long)simpleUserId } };

        return await dbCon.QueryAsync<UserTermFavoriteObject.Raw>(
            sql,
            new DynamicParameters(sqlParams)
        );
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

        var dataTable = new DataTable();
        dataTable.Columns.Add(TableColumn_SimpleUserId, typeof(long));
        dataTable.Columns.Add(TableColumn_FavTermId, typeof(long));
        dataTable.Columns.Add(TableColumn_Favor, typeof(int));

        for( int i=0; i<favTermIds.Length; i++ ) {
            dataTable.Rows.Add( (long)simpleUserId, (long)favTermIds[i], 0 );
        }

        using( SqlBulkCopy bulkCopy = new SqlBulkCopy((SqlConnection)dbCon) ) {
            bulkCopy.DestinationTableName = TableName; 

            await bulkCopy.WriteToServerAsync( dataTable );
        }
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
    }
}
