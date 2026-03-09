using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Threading;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Client.Services.DataAccess;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_SimplePosts : IClientDataAccess {
    public static async Task<SimplePostObject> ToObject(
                ClientDataAccess_Terms termsData,
                SimplePostObject.DatabaseEntry entryRaw ) {
        Func<long[], Task<TermObject[]>> termSetFactory = async ( long[] termSetIds ) => {
            IEnumerable<TermObject.DatabaseEntry> termRaws = (await termsData.GetByIds_Async( termSetIds ))
                .Terms;
            IEnumerable<Task<TermObject>> termWhens = termRaws
                .Select( async termRaw => await termRaw.CreateTermObject_Async( null ) );
            return (await Task.WhenAll(termWhens))
                .ToArray();
        };
        
        return await entryRaw.CreateSimplePost_Async( termSetFactory );
    }
}
