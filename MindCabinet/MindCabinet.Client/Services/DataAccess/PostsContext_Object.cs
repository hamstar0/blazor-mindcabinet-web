using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Threading;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects.PostsContext;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_PostsContext : IClientDataAccess {
    public static async Task<PostsContextObject[]> ConvertRawsToDataObjects_Async(
                ClientDataAccess_Terms termsData,
                PostsContextObject.Raw[] entriesRaw ) {
        return await Task.WhenAll(
            entriesRaw.Select( entryRaw => ClientDataAccess_PostsContext.ConvertRawToDataObject_Async(termsData, entryRaw) )
        );
    }

    public static async Task<PostsContextObject> ConvertRawToDataObject_Async(
                ClientDataAccess_Terms termsData,
                PostsContextObject.Raw ctxRaw ) {
        Func<PostsContextTermEntryObject.Raw[], Task<PostsContextTermEntryObject[]>> ctxTermEntriesFactory = 
            async ctxTermEntriesRaw => {
                return await ClientDataAccess_PostsContext.ConvertRawsToTermEntryDataObjects_Async(
                    termsData,
                    ctxTermEntriesRaw
                );
            };

        return await ctxRaw.ToDataObject_Async( ctxTermEntriesFactory );
    }


    public static async Task<PostsContextTermEntryObject[]> ConvertRawsToTermEntryDataObjects_Async(
                ClientDataAccess_Terms termsData,
                PostsContextTermEntryObject.Raw[] ctxTermEntriesRaw ) {
        TermId[] termIds = ctxTermEntriesRaw.Select( t => t.TermId ).ToArray();

        IEnumerable<TermObject.Raw> termsRaw = (await termsData.GetByIds_Async( termIds ))
            .Terms;

        Func<TermId, Task<TermObject>> termFactory = async termId => await ClientDataAccess_Terms
            .ConvertRawToDataObject_Async( termsData, termsRaw.First(termRaw => termRaw.Id == termId) );

        PostsContextTermEntryObject[] entries = await Task.WhenAll(
            ctxTermEntriesRaw.Select( ctxTermEntryRaw => ctxTermEntryRaw.ToDataObject_Async(termFactory) )
        );

        return entries;
    }
}
