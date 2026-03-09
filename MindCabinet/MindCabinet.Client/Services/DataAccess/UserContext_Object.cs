using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Threading;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects.UserContext;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_UserContext : IClientDataAccess {
    public static async Task<UserContextObject> ToObject(
                ClientDataAccess_Terms termsData,
                UserContextObject.DatabaseEntry entryRaw ) {
        Func<UserContextTermEntryObject.DatabaseEntry[], Task<UserContextTermEntryObject[]>> ctxTermEntriesFactory = 
            async ctxTermEntriesRaw => {
                return await ClientDataAccess_UserContext.ToTermEntryObjects( termsData, ctxTermEntriesRaw );
            };

        return await entryRaw.CreateUserContextObject_Async( ctxTermEntriesFactory );
    }


    public static async Task<UserContextTermEntryObject[]> ToTermEntryObjects(
                ClientDataAccess_Terms termsData,
                UserContextTermEntryObject.DatabaseEntry[] ctxTermEntriesRaw ) {
        long[] termIds = ctxTermEntriesRaw.Select( t => t.TermId ).ToArray();
        IEnumerable<TermObject.DatabaseEntry> termsRaw = (await termsData.GetByIds_Async( termIds ))
            .Terms;

        Func<long, Task<TermObject>> termFactory = async termId => await ClientDataAccess_Terms
            .ToObject( termsData, termId );

        return await Task.WhenAll(
            ctxTermEntriesRaw.Select( ctxTermEntryRaw => ctxTermEntryRaw.CreateUserContextTermEntry_Async(termFactory) )
        );
    }
}
