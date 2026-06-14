using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Threading;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects.PostsContext;
using System.Text.Json;
using Microsoft.AspNetCore.Components;


namespace MindCabinet.Client.Services.DbAccess.Joined;



public partial class ClientDataAccess_PrioritizedPosts : IClientDataAccess {
    private HttpClient Http;



    public ClientDataAccess_PrioritizedPosts( HttpClient http ) {
        this.Http = http;
    }


    public async Task<IEnumerable<SimplePostObject.Raw>> GetByCriteriaForCurrentUser_Async( IAPI.GetByCriteria_Params parameters ) {
        var ret = await IClientDataAccess.CallAPI_Async<IAPI.GetByCriteria_Params, IEnumerable<SimplePostObject.Raw>>(
            http: this.Http,
            route: $"{IAPI.BaseRoute}/{nameof(IAPI.GetByCriteriaForCurrentUser_Async)}",
            parameters: parameters
        );
        
        return ret;
    }

    
    public async Task<int> GetCountByCriteria_Async( IAPI.GetByCriteria_Params parameters ) {
        var ret = await IClientDataAccess.CallAPIManual_Async<IAPI.GetByCriteria_Params, int>(  // needed?
            http: this.Http,
            route: $"{IAPI.BaseRoute}/{nameof(IAPI.GetCountByCriteriaForCurrentUser_Async)}",
            parameters: parameters
        );
        
        return ret;
    }
}
