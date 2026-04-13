using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Threading;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects.PostsContext;
using MindCabinet.Shared.DataObjects.UserTermFavorite;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_UserTermFavorites : IClientDataAccess {
    public static async Task<UserTermFavoriteObject.ClientObject[]> ConvertRawsToClientObjects_Async(
                ClientDataAccess_Terms termsData,
                UserTermFavoriteObject.Raw[] entriesRaw ) {
        TermId[] termIds = entriesRaw.Select( t => t.FavTermId ).ToArray();
        IEnumerable<TermObject.Raw> termsRaw = (await termsData.GetByIds_Async( termIds ))
            .Terms;
        
        Func<TermId, Task<TermObject>> termFactory = async termId => await ClientDataAccess_Terms
            .ConvertRawToDataObject_Async( termsData, termsRaw.First(termRaw => termRaw.Id == termId) );

        return await Task.WhenAll(
            entriesRaw.Select( entryRaw => entryRaw.ToClientObject_Async(termFactory) )
        );
    }
}
