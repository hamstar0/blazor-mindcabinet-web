using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Threading;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects.PostsQuery;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_PostsQuery : IClientDataAccess {
    public static async Task<PostsQueryObject> ConvertRawToDataObject_Async(
                ClientDataAccess_Terms termsDataSrc,
                PostsQueryObject.Raw queryRaw ) {
        Func<PostsQueryTermEntryObject.Raw[], Task<PostsQueryTermEntryObject[]>> queryTermEntriesFactory = 
            async queryTermEntriesRaw => {
                return await ClientDataAccess_PostsQuery.ConvertRawsToTermEntryDataObjects_Async(
                    termsDataSrc,
                    queryTermEntriesRaw
                );
            };

        return await queryRaw.ToDataObject_Async( queryTermEntriesFactory );
    }

    public static async Task<PostsQueryObject[]> ConvertRawsToDataObjects_Async(
                ClientDataAccess_Terms termsDataSrc,
                PostsQueryObject.Raw[] entriesRaw ) {
        return await Task.WhenAll(
            entriesRaw.Select( entryRaw => ClientDataAccess_PostsQuery.ConvertRawToDataObject_Async(termsDataSrc, entryRaw) )
        );
    }


    public static async Task<PostsQueryTermEntryObject[]> ConvertRawsToTermEntryDataObjects_Async(
                ClientDataAccess_Terms termsDataSrc,
                PostsQueryTermEntryObject.Raw[] queryTermEntriesRaw ) {
        TermId[] termIds = queryTermEntriesRaw.Select( t => t.TermId ).ToArray();

        IEnumerable<TermObject.Raw> termsRaw = (await termsDataSrc.GetByIds_Async( termIds ))
            .Terms;

        Func<TermId, Task<TermObject>> termFactory = async termId => await ClientDataAccess_Terms
            .ConvertRawToDataObject_Async( termsDataSrc, termsRaw.First(termRaw => termRaw.Id == termId) );

        PostsQueryTermEntryObject[] entries = await Task.WhenAll(
            queryTermEntriesRaw.Select( queryTermEntryRaw => queryTermEntryRaw.ToDataObject_Async(termFactory) )
        );

        return entries;
    }
}
