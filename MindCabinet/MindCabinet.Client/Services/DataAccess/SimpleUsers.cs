using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects;


namespace MindCabinet.Client.Services.DbAccess;


public partial class ClientDataAccess_SimpleUsers : IClientDataAccess {
    private HttpClient Http;



    public ClientDataAccess_SimpleUsers( HttpClient http ) {
        this.Http = http;
    }


    public async Task<IAPI.Create_Return> Create_Async( IAPI.Create_Params parameters ) {
        var ret = await IClientDataAccess.CallAPI_Async<IAPI.Create_Params, IAPI.Create_Return>(
            http: this.Http,
            route: $"{IAPI.BaseRoute}/{nameof(IAPI.Create_Async)}",
            parameters: parameters
        );
        
        return ret;
    }


    public async Task<IAPI.Login_Return> Login_Async( IAPI.Login_Params parameters ) {
        var ret = await IClientDataAccess.CallAPI_Async<IAPI.Login_Params, IAPI.Login_Return>(
            http: this.Http,
            route: $"{IAPI.BaseRoute}/{nameof(IAPI.Login_Async)}",
            parameters: parameters
        );
        
        return ret;
    }
}
