using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserTermFavorite;

namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_UserTermFavorites : IClientDataAccess {
    private static IEnumerable<UserTermFavoriteObject.Raw>? Cache_ForCurrentUser = null;



    private HttpClient Http;

    private LocalClientSessionManager MySessionMngr;



    public ClientDataAccess_UserTermFavorites( HttpClient http, LocalClientSessionManager mySessionMngr ) {
        this.Http = http;
        this.MySessionMngr = mySessionMngr;
    }


    public async Task<IEnumerable<UserTermFavoriteObject.Raw>> GetFavTermsForCurrentUser_Async() {   //( Get_Params parameters ) {
        if( Cache_ForCurrentUser is not null ) {
            return Cache_ForCurrentUser;
        }

        //

        var ret = await IClientDataAccess.CallAPI_Async<IEnumerable<UserTermFavoriteObject.Raw>>(
            http: this.Http,
            route: $"{IAPI.BaseRoute}/{nameof(IAPI.GetFavTermsForCurrentUser_Async)}"
        );

        //

        Cache_ForCurrentUser = ret;

        return ret;
    }


    public async Task AddTermsForCurrentUser_Async( IAPI.AddTermsForCurrentUser_Params parameters ) {
        await IClientDataAccess.CallAPI_Async<IAPI.AddTermsForCurrentUser_Params>(
            http: this.Http,
            route: $"{IAPI.BaseRoute}/{nameof(IAPI.AddTermsForCurrentUser_Async)}",
            parameters: parameters
        );

        //

        Cache_ForCurrentUser = null;
    }


    public async Task RemoveTermsForCurrentUser_Async( IAPI.RemoveTermsForCurrentUser_Params parameters ) {
        await IClientDataAccess.CallAPI_Async<IAPI.RemoveTermsForCurrentUser_Params>(
            http: this.Http,
            route: $"{IAPI.BaseRoute}/{nameof(IAPI.RemoveTermsForCurrentUser_Async)}",
            parameters: parameters
        );

        //

        Cache_ForCurrentUser = null;
    }
}
