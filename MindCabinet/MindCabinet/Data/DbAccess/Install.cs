using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Dapper;
using static MindCabinet.Program;


namespace MindCabinet.Data.DbAccess;


public partial class ServerDbAccess {
    public async Task<bool> Install_Async( IDbConnection dbCon ) {
        bool success;
        long defaultUserId;

        (success, defaultUserId) = await this.InstallSimpleUsers_Async( dbCon );
        if( !success ) {
            return false;
        }
        success = await this.InstallSimpleUserSessions_Async( dbCon );
        if( !success ) {
            return false;
        }
        success = await this.InstallTerms_Async( dbCon );
        if( !success ) {
            return false;
        }
        success = await this.InstallSimplePosts_Async( dbCon, defaultUserId );
        if( !success ) {
            return false;
        }

        return true;
    }
}
