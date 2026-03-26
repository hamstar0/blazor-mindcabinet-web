using Dapper;
using Microsoft.Data.SqlClient;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserPostsContext;
using MindCabinet.Shared.Utility;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_UserAppData : IServerDataAccess {
    public async static Task<UserAppDataObject> ToObject_Async(
                IDbConnection dbCon,
                ServerDataAccess_Terms termsData,
                ServerDataAccess_UserPostsContexts userPostsContextsData,
                UserAppDataObject.Raw dbEntry ) {
        Func<UserPostsContextTermEntryObject.Raw[], Task<UserPostsContextTermEntryObject[]>> ctxTermsFactory = async ctxTermEntries => {
            return await ServerDataAccess_UserPostsContexts.ToTermEntriesDataObjects_Async(
                dbCon,
                termsData,
                ctxTermEntries
            );
        };

        Func<UserPostsContextId, Task<UserPostsContextObject>> userPostsContextFactory = async id => {
            UserPostsContextObject.Raw? ctxRaw = await userPostsContextsData.GetById_Async( dbCon, id, true );
            if( ctxRaw is null ) {
                throw new Exception( $"UserPostsContext with id {id} not found." );
            }

            return await ctxRaw.CreateDataObject_Async( ctxTermsFactory );
        };

        return await dbEntry.CreateDataObject_Async( userPostsContextFactory );
    }
}
