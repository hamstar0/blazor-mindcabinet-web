using System.Net.Http.Json;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects;


namespace MindCabinet.Client.Services.DbAccess;


public partial class ClientDataAccess_SimpleUsers : IClientDataAccess {
    private HubConnection HubConnection;



    public ClientDataAccess_SimpleUsers( NavigationManager navigationManager ) {
        Uri hubUrl = navigationManager.ToAbsoluteUri( IAPI.BaseRoute );
        this.HubConnection = new HubConnectionBuilder()
            .WithUrl( hubUrl )
            .Build();
    }

    public async ValueTask DisposeAsync() {
        await this.HubConnection.DisposeAsync();
    }


    public async Task<IAPI.Create_Return> Create_Async( IAPI.Create_Params parameters ) {
        return await IClientDataAccess.CallHub_Async<IAPI.Create_Return>(
            hubConnection: this.HubConnection,
            methodName: nameof( IAPI.Create_Async ),
            args: new object[] { parameters }
        );
    }


    public async Task<IAPI.Login_Return> Login_Async( IAPI.Login_Params parameters ) {
        return await IClientDataAccess.CallHub_Async<IAPI.Login_Return>(
            hubConnection: this.HubConnection,
            methodName: nameof( IAPI.Login_Async ),
            args: new object[] { parameters }
        );
    }
}
