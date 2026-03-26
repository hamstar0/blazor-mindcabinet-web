using System.Data;
using Microsoft.Data.SqlClient;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserPostsContext;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_Install : IServerDataAccess {
    public async Task<bool> Install_Async(
                IDbConnection dbCon,
                ServerDataAccess_SimpleUsers simpleUsersData,
                ServerDataAccess_SimpleUserSessions sessionsData,
                ServerDataAccess_Terms termsData,
                ServerDataAccess_TermSets termSetsData,
                ServerDataAccess_SimplePosts simplePostsData,
                ServerDataAccess_UserFavoriteTerms favoriteTermsData,
                ServerDataAccess_UserPostsContexts userPostsContextData,
                ServerDataAccess_UserAppData userAppData ) {
        if( await DbAccess.IsInstalled(dbCon) ) {
            return true;
        }
        
        bool success;
        SimpleUserId defaultUserId;
        TermObject.Raw sampleTerm;
        UserPostsContextObject.Raw sampleUsrCtx;

        (success, defaultUserId) = await simpleUsersData.Install_Async( dbCon );
        if( !success ) {
            return false;
        }
        success = await sessionsData.Install_Async( dbCon );
        if( !success ) {
            return false;
        }
        success = await termsData.Install_Async( dbCon );
        if( !success ) {
            return false;
        }
        success = await simplePostsData.Install_Async( dbCon );
        if( !success ) {
            return false;
        }
        success = await termSetsData.Install_Async( dbCon );
        if( !success ) {
            return false;
        }
        (success, sampleTerm) = await simplePostsData.Install_AfterTermSets_Async( dbCon, termsData, termSetsData, defaultUserId );
        if( !success ) {
            return false;
        }
        success = await favoriteTermsData.Install_Async( dbCon );
        if( !success ) {
            return false;
        }
        (success, sampleUsrCtx) = await userPostsContextData.Install_Async( dbCon, sampleTerm, defaultUserId );
        if( !success ) {
            return false;
        }
        success = await userAppData.Install_Async( dbCon, defaultUserId, sampleUsrCtx );
        if( !success ) {
            return false;
        }

        return success;
    }
}
