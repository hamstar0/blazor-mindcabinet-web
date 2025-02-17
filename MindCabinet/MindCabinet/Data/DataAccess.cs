using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Dapper;
using static MindCabinet.Program;


namespace MindCabinet.Data;


public partial class ServerDataAccess {
    public class ServerDataAccessParameters {
        public string ConnectionString = "";
    }



    private SingletonCache Cache;
    private string ConnectionString;



    public ServerDataAccess( SingletonCache cache, IOptions<ServerDataAccessParameters> connectionString ) {
        this.Cache = cache;
        this.ConnectionString = connectionString.Value.ConnectionString;
    }

    public async Task<IDbConnection> ConnectDb( bool validateInstall=true ) {
        //using var con = new SqlConnection( this.ConnectionString );
        var dbCon = new SqlConnection( this.ConnectionString );
        dbCon.Open();

        if( validateInstall ) {
            int count = await dbCon.QuerySingleAsync<int>( @"
                SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_NAME = N'Posts'"
            );
            //int count = await dbCon.ExecuteAsync( @"
            //    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'Posts')
            //    BEGIN
            //        RETURN 1;
            //    END
            //    ELSE
            //    BEGIN
            //        RETURN 0;
            //    END" );
            if( count == 0 ) {
                throw new DataException( "Database not installed." );
            }
        }

        return dbCon;
    }


    public async Task<bool> Install_Async( IDbConnection dbCon ) {
        return await this.InstallTerms_Async( dbCon )
            && await this.InstallPosts_Async( dbCon );
    }
}
