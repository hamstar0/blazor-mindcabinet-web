using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Threading;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects.UserContext;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_Terms : IClientDataAccess {
    public static async Task<TermObject> ToObject_Async(
                ClientDataAccess_Terms termsData,
                long termId ) {
        Func<long, Task<TermObject.Raw>> termRawFactory = async (long termId) =>
            (await termsData.GetByIds_Async( new long[] { termId } ))
            .Terms
            .First();

        TermObject.Raw termRaw = await termRawFactory( termId );

        return await termRaw.CreateDataObject_Async( termRawFactory );
    }


    public static async Task<TermObject[]> ToObjects_Async(
                ClientDataAccess_Terms termsData,
                TermObject.Raw[] rawTerms ) {
        Func<long, Task<TermObject.Raw>> termRawFactory = async (long termId) =>
            (await termsData.GetByIds_Async( new long[] { termId } ))
            .Terms
            .First();

        return await Task.WhenAll(
            rawTerms.Select( async rawTerm => await rawTerm.CreateDataObject_Async(termRawFactory) )
        );
    }
}
