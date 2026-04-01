using Dapper;
using Microsoft.Data.SqlClient;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsContext;
using MindCabinet.Shared.Utility;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_UserAppData : IServerDataAccess {
    public async static Task<UserAppDataObject> ToObject_Async(
                IDbConnection dbCon,
                ServerDataAccess_Terms termsData,
                ServerDataAccess_PostsContexts postsContextsData,
                UserAppDataObject.Raw dbEntry ) {
        Func<PostsContextTermEntryObject.Raw[], Task<PostsContextTermEntryObject[]>> ctxTermsFactory = async ctxTermEntries => {
            return await ServerDataAccess_PostsContexts.ToTermEntriesDataObjects_Async(
                dbCon,
                termsData,
                ctxTermEntries
            );
        };

        Func<PostsContextId, Task<PostsContextObject>> postsContextFactory = async id => {
            PostsContextObject.Raw? ctxRaw = await postsContextsData.GetById_Async( dbCon, id, true );
            if( ctxRaw is null ) {
                throw new Exception( $"PostsContext with id {id} not found." );
            }

            return await ctxRaw.CreateDataObject_Async( ctxTermsFactory );
        };

        return await dbEntry.CreateDataObject_Async( postsContextFactory );
    }
}
