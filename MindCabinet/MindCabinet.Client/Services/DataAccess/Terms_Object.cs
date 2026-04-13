using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Threading;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects.PostsContext;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_Terms : IClientDataAccess {
    public static async Task<TermObject> ConvertRawToDataObject_Async(
                ClientDataAccess_Terms termsData,
                TermObject.Raw termRaw ) {
        Func<TermId, Task<TermObject.Raw>> termRawFactory = async (TermId termId) =>
            (await termsData.GetByIds_Async( new TermId[] { termId } ))
            .Terms
            .First();

        return await termRaw.ToDataObject_Async( termRawFactory );
    }


    public static async Task<TermObject[]> ConvertRawsToDataObjects_Async(
                ClientDataAccess_Terms termsData,
                TermObject.Raw[] rawTerms ) {
        Func<TermId, Task<TermObject.Raw>> termRawFactory = async (TermId termId) =>
            (await termsData.GetByIds_Async( new TermId[] { termId } ))
            .Terms
            .First();

        return await Task.WhenAll(
            rawTerms.Select( async rawTerm => await rawTerm.ToDataObject_Async(termRawFactory) )
        );
    }
}
