using System.Data;
using Microsoft.Data.SqlClient;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsContext;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_Install : IServerDataAccess {
    public async Task<bool> Install_Async(
                IDbConnection dbCon,
                ServerDataAccess_SimpleUsers simpleUsersDataSrc,
                ServerDataAccess_SimpleUserSessions sessionsDataSrc,
                ServerDataAccess_Terms termsDataSrc,
                ServerDataAccess_SimplePostTags simplePostTagsDataSrc,
                ServerDataAccess_SimplePosts simplePostsDataSrc,
                ServerDataAccess_UserTermFavorites favoriteTermsDataSrc,
                ServerDataAccess_UserTermsHistory historyTermsDataSrc,
                ServerDataAccess_PostsContexts postsContextDataSrc,
                ServerDataAccess_PostsContextTermEntry postsContextTermEntryDataSrc,
                ServerDataAccess_UserAppData userAppDataSrc,
                ServerDataAccess_ServerData serverDataSrc ) {
        if( await DbAccess.IsInstalled(dbCon) ) {
            return true;
        }
        
        bool success;
        SimpleUserId defaultUserId;
        TermId usersConceptTermId;
        TermId defaultUserAsTermId;
        TermObject.Raw sampleTerm;

        success = await simpleUsersDataSrc.Install_Async( dbCon );
        if( !success ) {
            return false;
        }

        success = await sessionsDataSrc.Install_Async( dbCon );
        if( !success ) {
            return false;
        }

        success = await termsDataSrc.Install_Async( dbCon );
        if( !success ) {
            return false;
        }

        success = await serverDataSrc.Install_Async( dbCon );
        if( !success ) {
            return false;
        }

        success = await simplePostsDataSrc.Install_Async( dbCon );
        if( !success ) {
            return false;
        }

        success = await simplePostTagsDataSrc.Install_Async( dbCon );
        if( !success ) {
            return false;
        }

        success = await favoriteTermsDataSrc.Install_Async( dbCon );
        if( !success ) {
            return false;
        }

        success = await historyTermsDataSrc.Install_Async( dbCon );
        if( !success ) {
            return false;
        }

        success = await postsContextDataSrc.Install_Async( dbCon );
        if( !success ) {
            return false;
        }

        success = await postsContextTermEntryDataSrc.Install_Async( dbCon );
        if( !success ) {
            return false;
        }

        success = await userAppDataSrc.Install_Async( dbCon );
        if( !success ) {
            return false;
        }

        //

        (success, defaultUserId) = await simpleUsersDataSrc.Install_After_Async(
            dbCon,
            termsDataSrc,
            postsContextDataSrc,
            postsContextTermEntryDataSrc,
            serverDataSrc,
            userAppDataSrc
        );

        (success, usersConceptTermId) = await termsDataSrc.Install_After_Async( dbCon, defaultUserId );

        success = await serverDataSrc.Install_After_Async( dbCon, usersConceptTermId );
        if( !success ) {
            return false;
        }

        (success, defaultUserAsTermId) = await simpleUsersDataSrc.Install_AfterDefaultUserAndServerData_Async(
            dbCon,
            termsDataSrc,
            postsContextDataSrc,
            postsContextTermEntryDataSrc,
            serverDataSrc,
            userAppDataSrc,
            defaultUserId
        );
        if( !success ) {
            return false;
        }

        (success, sampleTerm) = await simplePostsDataSrc.Install_AfterUser_Async(
            dbCon,
            termsDataSrc,
            simplePostTagsDataSrc,
            defaultUserId,
            defaultUserAsTermId
        );
        if( !success ) {
            return false;
        }

        return success;
    }
}
