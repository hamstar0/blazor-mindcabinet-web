using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Dapper;
using static MindCabinet.Program;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_Install {
    public async Task<bool> Install_Async(
                IDbConnection dbCon,
                ServerDataAccess_SimpleUsers simpleUsersData,
                ServerDataAccess_SimpleUsers_Sessions sessionsData,
                ServerDataAccess_Terms termsData,
                ServerDataAccess_Terms_Sets termSetsData,
                ServerDataAccess_SimplePosts simplePostsData,
                ServerDataAccess_UserFavoriteTerms favoriteTermsData ) {
        bool success;
        long defaultUserId;

        (success, defaultUserId) = await simpleUsersData.Install_Async( dbCon );
        if( !success ) {
            return false;
        }
        success = await sessionsData.Install_Async( dbCon );
        if( !success ) {
            return false;
        }
        success = await termsData.Install_Async( dbCon, termSetsData );
        if( !success ) {
            return false;
        }
        success = await simplePostsData.Install_Async( dbCon, termsData, termSetsData, defaultUserId );
        if( !success ) {
            return false;
        }
        success = await favoriteTermsData.Install_Async( dbCon );
        if( !success ) {
            return false;
        }

        return true;
    }
}
