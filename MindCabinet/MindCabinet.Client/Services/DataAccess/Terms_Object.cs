using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Threading;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects.UserContext;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_Terms : IClientDataAccess {
    public static async Task<TermObject> ToObject(
                ClientDataAccess_Terms termsData,
                long termId ) {
        TermObject.DatabaseEntry termRaw = (await termsData.GetByIds_Async( new long[] { termId } ))
            .Terms
            .First();

        return await termRaw.CreateTermObject_Async( null );
    }
}
