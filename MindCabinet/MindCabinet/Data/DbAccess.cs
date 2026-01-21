using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Dapper;
using static MindCabinet.Program;
using MindCabinet.Data.DataAccess;


namespace MindCabinet.Data;


public partial class DbAccess {
    private readonly ILogger<DbAccess> Logger;

    //private SingletonCache Cache;
    //private ISession Session;
    //private ServerSettings ServerSettings;
    //private IHttpContextAccessor Http;
    private readonly Func<IDbConnection> ConnFactory;

    private IDbConnection? DbConnectionCache = null;


    public DbAccess(
                ILogger<DbAccess> logger,
                // ServerSettings serverSettings,
                //IHttpContextAccessor httpContextAccessor,
                Func<IDbConnection> connFactory ) {
        this.Logger = logger;
        //SingletonCache cache
        //IHttpContextAccessor hca
        //this.SessionId = hca.HttpContext.Request.Cookies["SessionId"];
        //this.Session = hca.HttpContext!.Session;
        // this.ServerSettings = serverSettings;
        //this.Http = httpContextAccessor;
        this.ConnFactory = connFactory;
    }

    public async Task<IDbConnection> GetDbConnection_Async( bool validateInstall=true ) {
        if( this.DbConnectionCache is not null ) {
            return this.DbConnectionCache;
        }

        this.DbConnectionCache = this.ConnFactory();
        this.DbConnectionCache.Open();

        if( validateInstall ) {
            dynamic? result = await this.DbConnectionCache.QueryFirstOrDefaultAsync( $"SHOW TABLES LIKE '{ServerDataAccess_SimplePosts.TableName}';" );
            // int count = await dbCon.QuerySingleAsync<int>( @"
            //  SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES
            //  WHERE TABLE_NAME = 'Posts'"
            //int count = await dbCon.ExecuteAsync( @"
            //    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'Posts')
            //    BEGIN
            //        RETURN 1;
            //    END
            //    ELSE
            //    BEGIN
            //        RETURN 0;
            //    END" );
            // if( count == 0 ) {
            if( result is null ) {
                throw new DataException( "Database not installed." );
            }
        }

        return this.DbConnectionCache;
    }
}
