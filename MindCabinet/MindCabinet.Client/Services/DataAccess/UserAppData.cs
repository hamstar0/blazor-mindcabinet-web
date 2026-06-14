using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Threading;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects.PostsContext;
using Microsoft.AspNetCore.Components;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_UserAppData : IClientDataAccess {
    public static UserAppDataObject.Raw? Cache_ForCurrentUser { get; private set; } = null;



    private HttpClient Http;



    public ClientDataAccess_UserAppData( HttpClient http ) {
        this.Http = http;
    }


    public async Task<IAPI.GetForCurrentUser_Return> GetForCurrentUser_Async() {
        if( Cache_ForCurrentUser is not null ) {
            return new IAPI.GetForCurrentUser_Return { UserAppData = Cache_ForCurrentUser };
        }

        //

        var ret = await IClientDataAccess.CallAPI_Async<IAPI.GetForCurrentUser_Return>(
            http: this.Http,
            route: $"{IAPI.BaseRoute}/{nameof(IAPI.GetForCurrentUser_Async)}"
        );

        //

        Cache_ForCurrentUser = ret.UserAppData;

        //

        return ret;
    }


    public async Task UpdateForCurrentUser_Async( UserAppDataObject.Prototype parameters ) {
        await IClientDataAccess.CallAPI_Async<UserAppDataObject.Prototype>(
            http: this.Http,
            route: $"{IAPI.BaseRoute}/{nameof(IAPI.UpdateForCurrentUser_Async)}",
            parameters: parameters
        );

        //

        Cache_ForCurrentUser = null;
    }
}
