using Dapper;
using Microsoft.Data.SqlClient;
using MindCabinet.Client.Services;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.Utility;
using System.Data;


namespace MindCabinet.Data;


public partial class ServerDbAccess {
    public async Task<bool> InstallSimpleUserFavoriteTags_Async( IDbConnection dbConnection ) {
        await dbConnection.ExecuteAsync( @"
            CREATE TABLE SimpleUserFavoriteTerms (
                SimpleUserId BIGINT NOT NULL,
                FavTermId BIGINT NOT NULL,
                PRIMARY KEY (SimpleUserId, FavTermId),
                CONSTRAINT FK_SessUserId FOREIGN KEY (SimpleUserId)
                    REFERENCES SimpleUsers(Id),
                CONSTRAINT FK_FavTermId FOREIGN KEY (FavTermId)
                    REFERENCES Terms(Id)
            );"
        //    ON DELETE CASCADE
        //    ON UPDATE CASCADE
        );

        return true;
    }
    

    public async Task<IEnumerable<long>> GetFavoriteTermIds_Async(
                IDbConnection dbCon,
                ClientDbAccess.GetSimpleUserFavoriteTagIdsParams parameters ) {
        string sql = @"SELECT FavTermId FROM SimpleUserFavoriteTerms WHERE SimpleUserId = @UserId;";
        var sqlParams = new Dictionary<string, object> { { "@UserId", parameters.UserId } };

        return await dbCon.QueryAsync<long>( sql, new DynamicParameters(sqlParams) );
	}


    public async Task AddSimpleUserFavoriteTermsById_Async(
                IDbConnection dbCon,
                ClientDbAccess.AddSimpleUserFavoriteTagsByIdParams parameters ) {
        var dataTable = new DataTable();
        dataTable.Columns.Add("SimpleUserId", typeof(long));
        dataTable.Columns.Add("FavTermId", typeof(long));

        for( int i=0; i<parameters.TermIds.Count; i++ ) {
            dataTable.Rows.Add( parameters.UserId, parameters.TermIds[i] );
        }

        using( SqlBulkCopy bulkCopy = new SqlBulkCopy((SqlConnection)dbCon) ) {
            bulkCopy.DestinationTableName = "SimpleUserFavoriteTerms"; 

            bulkCopy.WriteToServer( dataTable );
        }
    }
}
