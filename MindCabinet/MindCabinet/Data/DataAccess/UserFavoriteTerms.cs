using Dapper;
using Microsoft.Data.SqlClient;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.UserFavoriteTerm;
using MindCabinet.Shared.Utility;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_UserFavoriteTerms : IServerDataAccess {
    public const string TableName = "UserFavoriteTerms";

    public async Task<bool> Install_Async( IDbConnection dbConnection ) {
        await dbConnection.ExecuteAsync( $@"
            CREATE TABLE {TableName} (
                SimpleUserId BIGINT NOT NULL,
                FavTermId BIGINT NOT NULL,
                Favor INT NOT NULL,
                PRIMARY KEY (SimpleUserId, FavTermId),
                CONSTRAINT FK_{TableName}_SimpleUserId FOREIGN KEY (SimpleUserId)
                    REFERENCES {ServerDataAccess_SimpleUsers.TableName}(Id),
                CONSTRAINT FK_{TableName}_FavTermId FOREIGN KEY (FavTermId)
                    REFERENCES {ServerDataAccess_Terms.TableName}(Id),
                INDEX IDX_SimpleUserId (SimpleUserId),
                INDEX IDX_SimpleUserIdAndFavor (SimpleUserId, Favor)
            );"
        //    ON DELETE CASCADE
        //    ON UPDATE CASCADE
        );

        return true;
    }
    

    public async Task<IEnumerable<UserFavoriteTermObject.Raw>> GetFavTermEntries_Async(
                IDbConnection dbCon,
                long simpleUserId,
                ClientDataAccess_UserFavoriteTerms.GetTermIdsForCurrentUser_Params parameters ) {
        string sql = $"SELECT * FROM {TableName} WHERE SimpleUserId = @UserId;";
        var sqlParams = new Dictionary<string, object> { { "@UserId", simpleUserId } };

        return await dbCon.QueryAsync<UserFavoriteTermObject.Raw>(
            sql,
            new DynamicParameters(sqlParams)
        );
	}


    public async Task AddFavTermEntries_Async(
                IDbConnection dbCon,
                long simpleUserId,
                long[] favTermIds ) {
        var dataTable = new DataTable();
        dataTable.Columns.Add("SimpleUserId", typeof(long));
        dataTable.Columns.Add("FavTermId", typeof(long));
        dataTable.Columns.Add("Favor", typeof(int));

        for( int i=0; i<favTermIds.Length; i++ ) {
            dataTable.Rows.Add( simpleUserId, favTermIds[i], 0 );
        }

        using( SqlBulkCopy bulkCopy = new SqlBulkCopy((SqlConnection)dbCon) ) {
            bulkCopy.DestinationTableName = TableName; 

            bulkCopy.WriteToServer( dataTable );
        }
    }


    public async Task RemoveFavTermEntries_Async(
                IDbConnection dbCon,
                long simpleUserId,
                ClientDataAccess_UserFavoriteTerms.RemoveTermsForCurrentUser_Params parameters ) {
        await dbCon.ExecuteAsync(
            $@"DELETE FROM {TableName}
                WHERE SimpleUserId = @SimpleUserId
                    AND FavTermId IN @TermIds",
            new {
                SimpleUserId = simpleUserId,
                TermIds = parameters.TermIds
            }
        );
    }
}
