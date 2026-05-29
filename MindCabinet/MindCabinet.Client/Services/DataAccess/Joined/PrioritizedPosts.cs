using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Threading;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects.PostsContext;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR.Client;


namespace MindCabinet.Client.Services.DbAccess.Joined;



public partial class ClientDataAccess_PrioritizedPosts : IClientDataAccess {
    private HubConnection HubConnection;



    public ClientDataAccess_PrioritizedPosts() {
        this.HubConnection = new HubConnectionBuilder()
            .WithUrl( "/"+IAPI.BaseRoute )
            .Build();
    }

    public async ValueTask DisposeAsync() {
        await this.HubConnection.DisposeAsync();
    }


    public async Task<IEnumerable<SimplePostObject.Raw>> GetByCriteria_Async( IAPI.GetByCriteria_Params parameters ) {
        return await IClientDataAccess.CallHub<IEnumerable<SimplePostObject.Raw>>(
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
}
