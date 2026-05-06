using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Threading;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Client.Services.DataAccess;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_SimplePosts : IClientDataAccess {
    private static async Task<TermObject[]> GetTermObjectsFromIds(
                ClientDataAccess_Terms termsData,
                TermId[] termIds ) {
        TermObject.Raw[] termRaws = (await termsData.GetByIds_Async( termIds ))
            .Terms
            .ToArray();
        return await ClientDataAccess_Terms.ConvertRawsToDataObjects_Async( termsData, termRaws );
    }

    public static async Task<SimplePostObject> ConvertRawToDataObject_Async(
                ClientDataAccess_Terms termsData,
                SimplePostObject.Raw entryRaw ) {
        return await entryRaw.ToDataObject_Async(
            async ( TermId[] termIdsOfSet ) => await GetTermObjectsFromIds( termsData, termIdsOfSet )
        );
    }
}
