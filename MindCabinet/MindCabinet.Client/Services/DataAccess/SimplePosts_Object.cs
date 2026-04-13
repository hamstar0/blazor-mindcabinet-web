using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Threading;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Client.Services.DataAccess;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_SimplePosts : IClientDataAccess {
    public static async Task<SimplePostObject> ConvertRawToDataObject_Async(
                ClientDataAccess_Terms termsData,
                SimplePostObject.Raw entryRaw ) {
        Func<TermId[], Task<TermObject[]>> termSetFactory = async ( TermId[] termIdsOfSet ) => {
            TermObject.Raw[] termRaws = (await termsData.GetByIds_Async( termIdsOfSet ))
                .Terms
                .ToArray();
            return await ClientDataAccess_Terms.ConvertRawsToDataObjects_Async(termsData, termRaws);
        };
        
        return await entryRaw.ToDataObject_Async( termSetFactory );
    }
}
