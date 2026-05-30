using System.Net.Http.Json;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserTermFavorite;

namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_UserTermFavorites : IClientDataAccess {
    private HubConnection HubConnection;

    private LocalClientSessionManager MySessionMngr;



    public ClientDataAccess_UserTermFavorites( LocalClientSessionManager mySessionMngr, NavigationManager navigationManager ) {
        this.MySessionMngr = mySessionMngr;

        Uri hubUrl = navigationManager.ToAbsoluteUri( IAPI.BaseRoute );
        this.HubConnection = new HubConnectionBuilder()
            .WithUrl( hubUrl )
            .Build();
    }

    public async ValueTask DisposeAsync() {
        await this.HubConnection.DisposeAsync();
    }


    public async Task<IEnumerable<UserTermFavoriteObject.Raw>> GetFavTermsForCurrentUser_Async() {   //( Get_Params parameters ) {
        return await IClientDataAccess.CallHub_Async<IEnumerable<UserTermFavoriteObject.Raw>>(
            hubConnection: this.HubConnection,
            methodName: nameof( IAPI.GetFavTermsForCurrentUser_Async ),
            args: new object[] { new IAPI.GetFavTermsForCurrentUser_Params() }
        );
    }


    public async Task AddTermsForCurrentUser_Async( IAPI.AddTermsForCurrentUser_Params parameters ) {
        await IClientDataAccess.CallHub_Async<object>(
            hubConnection: this.HubConnection,
            methodName: nameof( IAPI.AddTermsForCurrentUser_Async ),
            args: new object[] { parameters }
        );
    }


    public async Task RemoveTermsForCurrentUser_Async( IAPI.RemoveTermsForCurrentUser_Params parameters ) {
        await IClientDataAccess.CallHub_Async<object>(
            hubConnection: this.HubConnection,
            methodName: nameof( IAPI.RemoveTermsForCurrentUser_Async ),
            args: new object[] { parameters }
        );
    }
}
