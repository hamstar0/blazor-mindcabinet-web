using System.Net.Http.Json;
using Microsoft.AspNetCore.SignalR.Client;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserTermHistory;

namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_UserTermsHistory : IClientDataAccess {
    private HubConnection HubConnection;

    private LocalClientSessionManager MySessionMngr;



    public ClientDataAccess_UserTermsHistory( LocalClientSessionManager mySessionMngr ) {
        this.MySessionMngr = mySessionMngr;
        this.HubConnection = new HubConnectionBuilder()
            .WithUrl( "/"+IAPI.BaseRoute )
            .Build();
    }

    public async ValueTask DisposeAsync() {
        await this.HubConnection.DisposeAsync();
    }


    public async Task<IEnumerable<UserTermHistoryObject.Raw>> GetHistTermsForCurrentUser_Async() {
        return await IClientDataAccess.CallHub<IEnumerable<UserTermHistoryObject.Raw>>(
            hubConnection: this.HubConnection,
            methodName: nameof( IAPI.GetHistTermsForCurrentUser_Async ),
            args: new object[] { }
        );
    }


    public async Task AddHistTermsForCurrentUser_Async( IAPI.AddHistTermsForCurrentUser_Params parameters ) {
        await IClientDataAccess.CallHub<object>(
            hubConnection: this.HubConnection,
            methodName: nameof( IAPI.AddHistTermsForCurrentUser_Async ),
            args: new object[] { parameters }
        );
    }
}
