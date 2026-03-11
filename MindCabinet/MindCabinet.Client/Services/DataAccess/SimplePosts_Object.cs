using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Threading;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Client.Services.DataAccess;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_SimplePosts : IClientDataAccess {
    public static async Task<SimplePostObject> ToObject_Async(
                ClientDataAccess_Terms termsData,
                SimplePostObject.Raw entryRaw ) {
        Func<long[], Task<TermObject[]>> termSetFactory = async ( long[] termSetIds ) => {
            TermObject.Raw[] termRaws = (await termsData.GetByIds_Async( termSetIds ))
                .Terms
                .ToArray();
            return await ClientDataAccess_Terms.ToObjects_Async(termsData, termRaws);
        };
        
        return await entryRaw.CreateDataObject_Async( termSetFactory );
    }
}
