using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Threading;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects.UserPostsContext;
using MindCabinet.Shared.DataObjects.UserFavoriteTerm;
using MindCabinet.Shared.DataObjects.UserHistoryTerm;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_UserTermsHistory : IClientDataAccess {
    public static async Task<UserHistoryTermObject.ClientObject[]> ToClientObjects_Async(
                ClientDataAccess_Terms termsData,
                UserHistoryTermObject.Raw[] entriesRaw ) {
        TermId[] termIds = entriesRaw.Select( t => t.TermId ).ToArray();
        IEnumerable<TermObject.Raw> termsRaw = (await termsData.GetByIds_Async( termIds ))
            .Terms;

        Func<TermId, Task<TermObject>> termFactory = async termId => await ClientDataAccess_Terms
            .ToObject_Async( termsData, termsRaw.First(termRaw => termRaw.Id == termId) );
        
        return await Task.WhenAll(
            entriesRaw.Select( entryRaw => entryRaw.CreateClientObject_Async(termFactory) )
        );
    }
}
