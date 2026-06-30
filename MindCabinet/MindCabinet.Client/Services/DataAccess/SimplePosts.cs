using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Threading;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Client.Services.DataAccess;
using Microsoft.AspNetCore.Components;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_SimplePosts : IClientDataAccess {
    private HttpClient Http;



    public ClientDataAccess_SimplePosts( HttpClient http ) {
        this.Http = http;
    }


    public async Task<IAPI.GetByCriteria_Return> GetByCriteria_Async( IAPI.GetByCriteria_Params parameters ) {
        var ret = await IClientDataAccess.CallAPI_Async<IAPI.GetByCriteria_Params, IAPI.GetByCriteria_Return>(
            http: this.Http,
            route: $"{IAPI.BaseRoute}/{nameof(IAPI.GetByCriteriaForCurrentUser_Async)}",
            parameters: parameters
        );

        return ret;
    }
    

    public async Task<int> GetCountByCriteria_Async( IAPI.GetByCriteria_Params parameters ) {
        int ret = await IClientDataAccess.CallAPIManual_Async<IAPI.GetByCriteria_Params, int>(  // needed?
            http: this.Http,
            route: $"{IAPI.BaseRoute}/{nameof(IAPI.GetCountByCriteriaForCurrentUser_Async)}",
            parameters: parameters
        );
        
        return ret;
    }


    public async Task<SimplePostObject.Raw> Create_Async( IAPI.Create_Params parameters ) {
        var ret = await IClientDataAccess.CallAPI_Async<IAPI.Create_Params, SimplePostObject.Raw>(
            http: this.Http,
            route: $"{IAPI.BaseRoute}/{nameof(IAPI.Create_Async)}",
            parameters: parameters
        );
        
        return ret;
    }
}
