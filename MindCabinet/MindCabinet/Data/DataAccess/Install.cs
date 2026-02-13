using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Dapper;
using static MindCabinet.Program;
using MindCabinet.Shared.DataObjects.Term;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_Install : IServerDataAccess {
    public async Task<bool> Install_Async(
                IDbConnection dbCon,
                ServerDataAccess_SimpleUsers simpleUsersData,
                ServerDataAccess_SimpleUsers_Sessions sessionsData,
                ServerDataAccess_Terms termsData,
                ServerDataAccess_Terms_Sets termSetsData,
                ServerDataAccess_SimplePosts simplePostsData,
                ServerDataAccess_UserFavoriteTerms favoriteTermsData,
                ServerDataAccess_UserContext userContextData ) {
        if( await DbAccess.IsInstalled(dbCon) ) {
            return true;
        }
        
        bool success;
        long defaultUserId;
        TermObject sampleTerm;

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
        (success, sampleTerm) = await simplePostsData.Install_Async( dbCon, termsData, termSetsData, defaultUserId );
        if( !success ) {
            return false;
        }
        success = await favoriteTermsData.Install_Async( dbCon );
        if( !success ) {
            return false;
        }
        success = await userContextData.Install_Async( dbCon, sampleTerm, defaultUserId );
        if( !success ) {
            return false;
        }

        return success;
    }
}
