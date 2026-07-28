using Dapper;
using Microsoft.Data.SqlClient;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsQuery;
using MindCabinet.Shared.Utility;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_UserAppData : IServerDataAccess {
    public async static Task<UserAppDataObject> ToDataObject_Async(
                IDbConnection dbCon,
                ServerDataAccess_Terms termsDataSrc,
                ServerDataAccess_PostsQuery postsQueryDataSrc,
                ServerDataAccess_PostsQueryTermEntry postsQueryTermEntryDataSrc,
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

        Func<PostsQueryTermEntryObject.Raw[], Task<PostsQueryTermEntryObject[]>> queryTermsFactory = async queryTermEntries => {
            return await ServerDataAccess_PostsQuery.ToTermEntriesDataObjects_Async(
                dbCon,
                termsDataSrc,
                queryTermEntries
            );
        };

        Func<PostsQueryId, Task<PostsQueryObject>> postsQueryFactory = async id => {
            PostsQueryObject.Raw? queryRaw = await postsQueryDataSrc.GetById_Async(
                dbCon: dbCon,
                postsQueryTermEntryDataSrc: postsQueryTermEntryDataSrc,
                postsQueryId: id,
                alsoGetEntries: true
            );
            if( queryRaw is null ) {
                throw new Exception( $"PostsQuery with id {id} not found." );
            }

            return await queryRaw.ToDataObject_Async( queryTermsFactory );
        };

        return await dbEntry.ToDataObject_Async( postsQueryFactory, termsFactory );
    }
}
