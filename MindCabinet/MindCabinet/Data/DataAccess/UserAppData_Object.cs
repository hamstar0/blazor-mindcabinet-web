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
                ServerDataAccess_Terms termsDataSrc,
                ServerDataAccess_PostsContexts postsContextsDataSrc,
                ServerDataAccess_PostsContextTermEntry postsContextTermEntryDataSrc,
                UserAppDataObject.Raw dbEntry ) {
        Func<TermId, Task<TermObject.Raw>> termsRawFactory = async id => {
            TermObject.Raw? termRaw = await termsDataSrc.GetById_Async( dbCon, id );
            if( termRaw is null ) {
                throw new Exception( $"Term with id {id} not found." );
            }

            return termRaw;
        };

        Func<TermId, Task<TermObject>> termsFactory = async id => {
            TermObject.Raw? termRaw = await termsRawFactory( id );

            return await termRaw.ToDataObject_Async( termsRawFactory );
        };

        Func<PostsContextTermEntryObject.Raw[], Task<PostsContextTermEntryObject[]>> ctxTermsFactory = async ctxTermEntries => {
            return await ServerDataAccess_PostsContexts.ToTermEntriesDataObjects_Async(
                dbCon,
                termsDataSrc,
                ctxTermEntries
            );
        };

        Func<PostsContextId, Task<PostsContextObject>> postsContextFactory = async id => {
            PostsContextObject.Raw? ctxRaw = await postsContextsDataSrc.GetById_Async( dbCon, postsContextTermEntryDataSrc, id, true );
            if( ctxRaw is null ) {
                throw new Exception( $"PostsContext with id {id} not found." );
            }

            return await ctxRaw.ToDataObject_Async( ctxTermsFactory );
        };

        return await dbEntry.ToDataObject_Async( postsContextFactory, termsFactory );
    }
}
