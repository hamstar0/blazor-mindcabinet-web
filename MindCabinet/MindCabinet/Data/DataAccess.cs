using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;


namespace MindCabinet.Data;


public partial class ServerDataAccess {
    private string ConnectionString;

    public ServerDataAccess( string connectionString ) {
        this.ConnectionString = connectionString;
    }

    private async Task<IDbConnection> ConnectDb( bool validateInstall=true ) {
        //using var con = new SqlConnection( this.ConnectionString );
        var con = new SqlConnection( this.ConnectionString );
        con.Open();

        if( validateInstall ) {
            int count = await con.ExecuteAsync( @"
                IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'Posts')
                BEGIN
                    RETURN 1;
                END
                ELSE
                BEGIN
                    RETURN 0;
                END" );
            if( count == 0 ) {
                throw new DataException( "Database not installed." );
            }
        }

        return con;
    }


    public async Task<bool> Install_Async() {
        using IDbConnection dbCon = await this.ConnectDb( false );

        return await this.InstallTerms_Async( dbCon )
            && await this.InstallPosts_Async( dbCon );
    }
}
