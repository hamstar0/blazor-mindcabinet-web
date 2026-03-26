using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Threading;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects.UserPostsContext;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_UserPostsContext : IClientDataAccess {
    public static async Task<UserPostsContextObject[]> ToObjects_Async(
                ClientDataAccess_Terms termsData,
                UserPostsContextObject.Raw[] entriesRaw ) {
        return await Task.WhenAll(
            entriesRaw.Select( entryRaw => ClientDataAccess_UserPostsContext.ToObject_Async(termsData, entryRaw) )
        );
    }

    public static async Task<UserPostsContextObject> ToObject_Async(
                ClientDataAccess_Terms termsData,
                UserPostsContextObject.Raw entryRaw ) {
        Func<UserPostsContextTermEntryObject.Raw[], Task<UserPostsContextTermEntryObject[]>> ctxTermEntriesFactory = 
            async ctxTermEntriesRaw => {
                return await ClientDataAccess_UserPostsContext.ToTermEntryObjects_Async( termsData, ctxTermEntriesRaw );
            };

        return await entryRaw.CreateDataObject_Async( ctxTermEntriesFactory );
    }


    public static async Task<UserPostsContextTermEntryObject[]> ToTermEntryObjects_Async(
                ClientDataAccess_Terms termsData,
                UserPostsContextTermEntryObject.Raw[] ctxTermEntriesRaw ) {
        TermId[] termIds = ctxTermEntriesRaw.Select( t => t.TermId ).ToArray();
        IEnumerable<TermObject.Raw> termsRaw = (await termsData.GetByIds_Async( termIds ))
            .Terms;

        Func<TermId, Task<TermObject>> termFactory = async termId => await ClientDataAccess_Terms
            .ToObject_Async( termsData, termsRaw.First(termRaw => termRaw.Id == termId) );

        return await Task.WhenAll(
            ctxTermEntriesRaw.Select( ctxTermEntryRaw => ctxTermEntryRaw.CreateDataObject_Async(termFactory) )
        );
    }
}
