using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserTermHistory;

namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_UserTermsHistory : IClientDataAccess {
    private static IEnumerable<UserTermHistoryObject.Raw>? Cache_ForCurrentUser = null;



    private HttpClient Http;

    private LocalClientSessionManager MySessionMngr;



    public ClientDataAccess_UserTermsHistory( HttpClient http, LocalClientSessionManager mySessionMngr ) {
        this.Http = http;
        this.MySessionMngr = mySessionMngr;
    }


    public async Task<IEnumerable<UserTermHistoryObject.Raw>> GetHistTermsForCurrentUser_Async() {
        if( Cache_ForCurrentUser is not null ) {
            return Cache_ForCurrentUser;
        }

        //

        var ret = await IClientDataAccess.CallAPI_Async<IEnumerable<UserTermHistoryObject.Raw>>(
            http: this.Http,
            route: $"{IAPI.BaseRoute}/{nameof(IAPI.GetHistTermsForCurrentUser_Async)}"
        );

        //

        Cache_ForCurrentUser = ret;

        return Cache_ForCurrentUser;
    }


    public async Task AddHistTermsForCurrentUser_Async( IAPI.AddHistTermsForCurrentUser_Params parameters ) {
        await IClientDataAccess.CallAPI_Async<IAPI.AddHistTermsForCurrentUser_Params>(
            http: this.Http,
            route: $"{IAPI.BaseRoute}/{nameof(IAPI.AddHistTermsForCurrentUser_Async)}",
            parameters: parameters
        );

        //

        Cache_ForCurrentUser = null;
    }
}
