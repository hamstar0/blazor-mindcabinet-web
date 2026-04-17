using System.Data;
using Microsoft.Data.SqlClient;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsContext;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_Install : IServerDataAccess {
    public async Task<bool> Install_Async(
                IDbConnection dbCon,
                ServerDataAccess_SimpleUsers simpleUsersData,
                ServerDataAccess_SimpleUserSessions sessionsData,
                ServerDataAccess_Terms termsData,
                ServerDataAccess_TermSets termSetsData,
                ServerDataAccess_SimplePosts simplePostsData,
                ServerDataAccess_UserTermFavorites favoriteTermsData,
                ServerDataAccess_UserTermsHistory historyTermsData,
                ServerDataAccess_PostsContexts postsContextData,
                ServerDataAccess_UserAppData userAppData,
                ServerDataAccess_ServerData serverData ) {
        if( await DbAccess.IsInstalled(dbCon) ) {
            return true;
        }
        
        bool success;
        SimpleUserId defaultUserId;
        TermId userConceptTermId;
        TermObject.Raw sampleTerm;
        PostsContextObject.Raw sampleUsrCtx;

        success = await simpleUsersData.Install_Async( dbCon );
        if( !success ) {
            return false;
        }
        success = await sessionsData.Install_Async( dbCon );
        if( !success ) {
            return false;
        }
        (success, userConceptTermId) = await termsData.Install_Async( dbCon );
        if( !success ) {
            return false;
        }
        (success, defaultUserId) = await simpleUsersData.Install_AfterTerms_Async( dbCon );
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
        success = await historyTermsData.Install_Async( dbCon );
        if( !success ) {
            return false;
        }
        (success, sampleUsrCtx) = await postsContextData.Install_Async( dbCon, sampleTerm );
        if( !success ) {
            return false;
        }
        success = await userAppData.Install_Async( dbCon, defaultUserId, sampleUsrCtx.Id );
        if( !success ) {
            return false;
        }
        success = await serverData.Install_Async( dbCon, userConceptTermId );
        if( !success ) {
            return false;
        }

        return success;
    }
}
