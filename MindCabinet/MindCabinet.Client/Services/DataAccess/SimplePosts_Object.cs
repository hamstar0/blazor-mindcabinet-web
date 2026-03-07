using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Threading;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Client.Services.DataAccess;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_SimplePosts : IClientDataAccess {
    public async Task<SimplePostObject?> ToObject( SimplePostObject.DatabaseEntry entry ) {
        Func<long, Task<TermSetObject>> termSetFactory = async ( long termSetId ) => {
            TermSetObject.DatabaseEntry? termSetEntry = await this.TermSetsData.GetById_Async( termSetId );
            return termSetEntry is not null ? await termSetEntry.CreateTermSetObject_Async() : null;
        };
        
        return await entry.CreateSimplePost_Async( termSetFactory );
    }
}
