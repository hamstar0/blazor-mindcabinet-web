using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Threading;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Client.Services.DataAccess;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_SimplePosts : IClientDataAccess {
    private static async Task<TermObject[]> GetTermObjectsFromIds(
                ClientDataAccess_Terms termsDataSrc,
                TermId[] termIds ) {
        TermObject.Raw[] termRaws = (await termsDataSrc.GetByIds_Async( termIds ))
            .Terms
            .ToArray();
        return await ClientDataAccess_Terms.ConvertRawsToDataObjects_Async( termsDataSrc, termRaws );
    }

    public static async Task<SimplePostObject> ConvertRawToDataObject_Async(
                ClientDataAccess_Terms termsDataSrc,
                SimplePostObject.Raw entryRaw ) {
        return await entryRaw.ToDataObject_Async(
            async ( TermId[] termIdsOfSet ) => await GetTermObjectsFromIds( termsDataSrc, termIdsOfSet )
        );
    }
}
