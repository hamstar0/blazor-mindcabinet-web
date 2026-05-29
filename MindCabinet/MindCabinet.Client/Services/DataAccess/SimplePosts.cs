using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Threading;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Client.Services.DataAccess;
using Microsoft.AspNetCore.SignalR.Client;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_SimplePosts : IClientDataAccess {
    private HubConnection HubConnection;



    public ClientDataAccess_SimplePosts() {
        this.HubConnection = new HubConnectionBuilder()
            .WithUrl( "/"+IAPI.BaseRoute )
            .Build();
    }

    public async ValueTask DisposeAsync() {
        await this.HubConnection.DisposeAsync();
    }


    public async Task<IAPI.GetByCriteria_Return> GetByCriteria_Async( IAPI.GetByCriteria_Params parameters ) {
        return await IClientDataAccess.CallHub<IAPI.GetByCriteria_Return>(
            hubConnection: this.HubConnection,
            methodName: nameof( IAPI.GetByCriteria_Async ),
            args: new object[] { parameters }
        );
    }
    

    public async Task<int> GetCountByCriteria_Async( IAPI.GetByCriteria_Params parameters ) {
        return await IClientDataAccess.CallHub<int>(
            hubConnection: this.HubConnection,
            methodName: nameof( IAPI.GetCountByCriteria_Async ),
            args: new object[] { parameters }
        );
    }


    public async Task<SimplePostObject.Raw> Create_Async( IAPI.Create_Params parameters ) {
        return await IClientDataAccess.CallHub<SimplePostObject.Raw>(
            hubConnection: this.HubConnection,
            methodName: nameof( IAPI.Create_Async ),
            args: new object[] { parameters }
        );
    }
}
