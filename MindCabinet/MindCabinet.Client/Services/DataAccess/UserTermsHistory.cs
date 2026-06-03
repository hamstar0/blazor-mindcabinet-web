using System.Net.Http.Json;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserTermHistory;

namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_UserTermsHistory : IClientDataAccess {
    private static IEnumerable<UserTermHistoryObject.Raw>? Cache_ForCurrentUser = null;



    private HubConnection HubConnection;

    private LocalClientSessionManager MySessionMngr;



    public ClientDataAccess_UserTermsHistory( LocalClientSessionManager mySessionMngr, NavigationManager navigationManager ) {
        this.MySessionMngr = mySessionMngr;

        Uri hubUrl = navigationManager.ToAbsoluteUri( IAPI.BaseRoute );
        this.HubConnection = new HubConnectionBuilder()
            .WithUrl( hubUrl )
            .Build();
    }

    public async ValueTask DisposeAsync() {
        await this.HubConnection.DisposeAsync();
    }


    public async Task<IEnumerable<UserTermHistoryObject.Raw>> GetHistTermsForCurrentUser_Async() {
        if( Cache_ForCurrentUser is not null ) {
            return Cache_ForCurrentUser;
        }

        //

        Cache_ForCurrentUser = await IClientDataAccess.CallHub_Async<IEnumerable<UserTermHistoryObject.Raw>>(
            hubConnection: this.HubConnection,
            methodName: nameof( IAPI.GetHistTermsForCurrentUser_Async ),
            args: new object[] { }
        );

        return Cache_ForCurrentUser;
    }


    public async Task AddHistTermsForCurrentUser_Async( IAPI.AddHistTermsForCurrentUser_Params parameters ) {
        await IClientDataAccess.CallHub_Async<object>(
            hubConnection: this.HubConnection,
            methodName: nameof( IAPI.AddHistTermsForCurrentUser_Async ),
            args: new object[] { parameters }
        );

        //

        Cache_ForCurrentUser = null;
    }
}
