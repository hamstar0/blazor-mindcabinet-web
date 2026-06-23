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
                ServerDataAccess_SimplePostTags simplePostTagsData,
                ServerDataAccess_SimplePosts simplePostsData,
                ServerDataAccess_UserTermFavorites favoriteTermsData,
                ServerDataAccess_UserTermsHistory historyTermsData,
                ServerDataAccess_PostsContexts postsContextData,
                ServerDataAccess_PostsContextTermEntry postsContextTermEntryData,
                ServerDataAccess_UserAppData userAppData,
                ServerDataAccess_ServerData serverData ) {
        if( await DbAccess.IsInstalled(dbCon) ) {
            return true;
        }
        
        bool success;
        SimpleUserId defaultUserId;
        TermId usersConceptTermId;
        TermId defaultUserAsTermId;
        TermObject.Raw sampleTerm;

        success = await simpleUsersData.Install_Async( dbCon );
        if( !success ) {
            return false;
        }

        success = await sessionsData.Install_Async( dbCon );
        if( !success ) {
            return false;
        }

        (success, usersConceptTermId) = await termsData.Install_Async( dbCon );
        if( !success ) {
            return false;
        }

        success = await serverData.Install_Async( dbCon );
        if( !success ) {
            return false;
        }

        success = await simplePostsData.Install_Async( dbCon );
        if( !success ) {
            return false;
        }

        success = await simplePostTagsData.Install_Async( dbCon );
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

        success = await postsContextData.Install_Async( dbCon );
        if( !success ) {
            return false;
        }

        success = await postsContextTermEntryData.Install_Async( dbCon );
        if( !success ) {
            return false;
        }

        success = await userAppData.Install_Async( dbCon );
        if( !success ) {
            return false;
        }

        //

        success = await serverData.Install_After_Async( dbCon, usersConceptTermId );
        if( !success ) {
            return false;
        }

        (success, defaultUserId, defaultUserAsTermId) = await simpleUsersData.Install_After_Async(
            dbCon,
            termsData,
            postsContextData,
            postsContextTermEntryData,
            serverData,
            userAppData
        );
        if( !success ) {
            return false;
        }

        (success, sampleTerm) = await simplePostsData.Install_AfterUser_Async(
            dbCon,
            termsData,
            simplePostTagsData,
            defaultUserId,
            defaultUserAsTermId
        );
        if( !success ) {
            return false;
        }

        return success;
    }
}
