using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Threading;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects.PostsContext;
using Microsoft.AspNetCore.SignalR.Client;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_UserAppData : IClientDataAccess {
    private HubConnection HubConnection;



    public ClientDataAccess_UserAppData() {
        this.HubConnection = new HubConnectionBuilder()
            .WithUrl( "/"+IAPI.BaseRoute )
            .Build();
    }

    public async ValueTask DisposeAsync() {
        await this.HubConnection.DisposeAsync();
    }

    public async Task<IAPI.GetForCurrentUser_Return> GetForCurrentUser_Async() {
        return await IClientDataAccess.CallHub<IAPI.GetForCurrentUser_Return>(
            hubConnection: this.HubConnection,
            methodName: nameof( IAPI.GetForCurrentUser_Async ),
            args: new object[] { }
        );
    }


    public async Task UpdateForCurrentUser_Async( UserAppDataObject.Prototype parameters ) {
        await IClientDataAccess.CallHub<object>(
            hubConnection: this.HubConnection,
            methodName: nameof( IAPI.UpdateForCurrentUser_Async ),
            args: new object[] { parameters }
        );
    }
}
