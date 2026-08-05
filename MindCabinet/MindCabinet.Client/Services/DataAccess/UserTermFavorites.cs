using System.Net.Http.Json;
using System.Text.Json;
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


    public async Task<IEnumerable<UserTermFavoriteObject.Raw>> GetFavTermsForCurrentUser_Async() {
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


    public async Task AddTermsForCurrentUser_Async( Dictionary<TermId, int> termIdToFavor ) {
        await IClientDataAccess.CallAPI_Async<IAPI.EditForCurrentUser_Params>(
            http: this.Http,
            route: $"{IAPI.BaseRoute}/{nameof(IAPI.AddTermsForCurrentUser_Async)}",
            parameters: new IAPI.EditForCurrentUser_Params {
                TermIds = termIdToFavor.Keys.ToArray(),
                TermFavors = termIdToFavor.Values.ToArray()
            }
        );

        //

        Cache_ForCurrentUser = null;
    }


    public async Task RemoveTermsForCurrentUser_Async( TermId[] TermIds ) {
        await IClientDataAccess.CallAPI_Async<IAPI.RemoveForCurrentUser_Params>(
            http: this.Http,
            route: $"{IAPI.BaseRoute}/{nameof(IAPI.RemoveTermsForCurrentUser_Async)}",
            parameters: new IAPI.RemoveForCurrentUser_Params {
                TermIds = TermIds
            }
        );

        //

        Cache_ForCurrentUser = null;
    }


    public async Task UpdateTermsForCurrentUser_Async( Dictionary<TermId, int> termIdToFavor ) {
        await IClientDataAccess.CallAPI_Async<IAPI.EditForCurrentUser_Params>(
            http: this.Http,
            route: $"{IAPI.BaseRoute}/{nameof(IAPI.UpdateTermsForCurrentUser_Async)}",
            parameters: new IAPI.EditForCurrentUser_Params {
                TermIds = termIdToFavor.Keys.ToArray(),
                TermFavors = termIdToFavor.Values.ToArray()
            }
        );

        //

        Cache_ForCurrentUser = null;
    }


    public async Task IncrementFavorForTerm_Async( TermId termId ) {
        await IClientDataAccess.CallAPI_Async<IAPI.IncrementFavorForTerm_Params>(
            http: this.Http,
            route: $"{IAPI.BaseRoute}/{nameof(IAPI.IncrementFavorForTerm_Async)}",
            parameters: new IAPI.IncrementFavorForTerm_Params { TermId = termId }
        );

        //

        Cache_ForCurrentUser = null;
    }
}
