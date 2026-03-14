using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Threading;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects.UserContext;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_UserContext : IClientDataAccess {
    public static async Task<UserContextObject[]> ToObjects_Async(
                ClientDataAccess_Terms termsData,
                UserContextObject.Raw[] entriesRaw ) {
        return await Task.WhenAll(
            entriesRaw.Select( entryRaw => ClientDataAccess_UserContext.ToObject_Async(termsData, entryRaw) )
        );
    }

    public static async Task<UserContextObject> ToObject_Async(
                ClientDataAccess_Terms termsData,
                UserContextObject.Raw entryRaw ) {
        Func<UserContextTermEntryObject.Raw[], Task<UserContextTermEntryObject[]>> ctxTermEntriesFactory = 
            async ctxTermEntriesRaw => {
                return await ClientDataAccess_UserContext.ToTermEntryObjects_Async( termsData, ctxTermEntriesRaw );
            };

        return await entryRaw.CreateDataObject_Async( ctxTermEntriesFactory );
    }


    public static async Task<UserContextTermEntryObject[]> ToTermEntryObjects_Async(
                ClientDataAccess_Terms termsData,
                UserContextTermEntryObject.Raw[] ctxTermEntriesRaw ) {
        long[] termIds = ctxTermEntriesRaw.Select( t => t.TermId ).ToArray();
        IEnumerable<TermObject.Raw> termsRaw = (await termsData.GetByIds_Async( termIds ))
            .Terms;

        Func<long, Task<TermObject>> termFactory = async termId => await ClientDataAccess_Terms
            .ToObject_Async( termsData, termsRaw.First(termRaw => termRaw.Id == termId) );

        return await Task.WhenAll(
            ctxTermEntriesRaw.Select( ctxTermEntryRaw => ctxTermEntryRaw.CreateDataObject_Async(termFactory) )
        );
    }
}
