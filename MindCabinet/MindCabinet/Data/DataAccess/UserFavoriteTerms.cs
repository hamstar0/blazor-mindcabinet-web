using Dapper;
using Microsoft.Data.SqlClient;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.Utility;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_UserFavoriteTerms {
    public const string TableName = "UserFavoriteTerms";

    public async Task<bool> Install_Async( IDbConnection dbConnection ) {
        await dbConnection.ExecuteAsync( $@"
            CREATE TABLE {TableName} (
                SimpleUserId BIGINT NOT NULL,
                FavTermId BIGINT NOT NULL,
                PRIMARY KEY (SimpleUserId, FavTermId),
                CONSTRAINT FK_SessUserId FOREIGN KEY (SimpleUserId)
                    REFERENCES {ServerDataAccess_SimpleUsers.TableName}(Id),
                CONSTRAINT FK_FavTermId FOREIGN KEY (FavTermId)
                    REFERENCES {ServerDataAccess_Terms.TableName}(Id)
            );"
        //    ON DELETE CASCADE
        //    ON UPDATE CASCADE
        );

        return true;
    }
    

    public async Task<IEnumerable<long>> GetTermIds_Async(
                IDbConnection dbCon,
                long simpleUserId,
                ClientDataAccess_UserFavoriteTerms.GetTermIdsForCurrentUser_Params parameters ) {
        string sql = $"SELECT FavTermId FROM {TableName} WHERE SimpleUserId = @UserId;";
        var sqlParams = new Dictionary<string, object> { { "@UserId", simpleUserId } };

        return await dbCon.QueryAsync<long>( sql, new DynamicParameters(sqlParams) );
	}


    public async Task AddTermIds_Async(
                IDbConnection dbCon,
                long simpleUserId,
                ClientDataAccess_UserFavoriteTerms.AddTermsForCurrentUser_Params parameters ) {
        var dataTable = new DataTable();
        dataTable.Columns.Add("SimpleUserId", typeof(long));
        dataTable.Columns.Add("FavTermId", typeof(long));

        for( int i=0; i<parameters.TermIds.Count; i++ ) {
            dataTable.Rows.Add( simpleUserId, parameters.TermIds[i] );
        }

        using( SqlBulkCopy bulkCopy = new SqlBulkCopy((SqlConnection)dbCon) ) {
            bulkCopy.DestinationTableName = TableName; 

            bulkCopy.WriteToServer( dataTable );
        }
    }


    public async Task RemoveTermIds_Async(
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
