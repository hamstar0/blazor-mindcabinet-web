using System.Net.Http.Json;
using Microsoft.AspNetCore.SignalR.Client;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects;


namespace MindCabinet.Client.Services.DbAccess;


public partial class ClientDataAccess_SimpleUsers : IClientDataAccess {
    private HubConnection HubConnection;



    public ClientDataAccess_SimpleUsers() {
        this.HubConnection = new HubConnectionBuilder()
            .WithUrl( "/"+IAPI.BaseRoute )
            .Build();
    }

    public async ValueTask DisposeAsync() {
        await this.HubConnection.DisposeAsync();
    }


    public async Task<IAPI.Create_Return> Create_Async( IAPI.Create_Params parameters ) {
        return await IClientDataAccess.CallHub<IAPI.Create_Return>(
            hubConnection: this.HubConnection,
            methodName: nameof( IAPI.Create_Async ),
            args: new object[] { parameters }
        );
    }


    public async Task<IAPI.Login_Return> Login_Async( IAPI.Login_Params parameters ) {
        return await IClientDataAccess.CallHub<IAPI.Login_Return>(
            hubConnection: this.HubConnection,
            methodName: nameof( IAPI.Login_Async ),
            args: new object[] { parameters }
        );
    }
}
