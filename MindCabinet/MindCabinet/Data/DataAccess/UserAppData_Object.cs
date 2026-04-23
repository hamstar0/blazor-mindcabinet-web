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
    public async static Task<UserAppDataObject> ToDataObject_Async(
                IDbConnection dbCon,
                ServerDataAccess_Terms termsData,
                ServerDataAccess_PostsContexts postsContextsData,
                ServerDataAccess_PostsContextTermEntry postsContextTermEntryData,
                UserAppDataObject.Raw dbEntry ) {
        Func<PostsContextTermEntryObject.Raw[], Task<PostsContextTermEntryObject[]>> ctxTermsFactory = async ctxTermEntries => {
            return await ServerDataAccess_PostsContexts.ToTermEntriesDataObjects_Async(
                dbCon,
                termsData,
                ctxTermEntries
            );
        };

        Func<PostsContextId, Task<PostsContextObject>> postsContextFactory = async id => {
            PostsContextObject.Raw? ctxRaw = await postsContextsData.GetById_Async( dbCon, postsContextTermEntryData, id, true );
            if( ctxRaw is null ) {
                throw new Exception( $"PostsContext with id {id} not found." );
            }

            return await ctxRaw.ToDataObject_Async( ctxTermsFactory );
        };

        return await dbEntry.ToDataObject_Async( postsContextFactory );
    }
}
