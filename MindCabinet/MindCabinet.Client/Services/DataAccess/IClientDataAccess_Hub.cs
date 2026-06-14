using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Threading;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using Microsoft.AspNetCore.SignalR.Client;


namespace MindCabinet.Client.Services.DataAccess;



public partial interface IClientDataAccess : IAsyncDisposable {
    public static async Task WaitForConnected_Async(
                HubConnection connection,
                TimeSpan? timeout = null,
                CancellationToken cancellationToken = default ) {
        var deadline = timeout is null
            ? DateTimeOffset.MaxValue
            : DateTimeOffset.UtcNow + timeout.Value;

        while( connection.State != HubConnectionState.Connected ) {
            if( connection.State == HubConnectionState.Disconnected ||
                connection.State == HubConnectionState.Reconnecting ) {
                throw new InvalidOperationException($"Connection could not reach Connected state. Current state: {connection.State}");
            }

            if( DateTimeOffset.UtcNow >= deadline ) {
                throw new TimeoutException("Timed out waiting for the hub connection to become Connected.");
            }

            await Task.Delay( 50, cancellationToken );
        }
    }


    public async static Task<TReturnType> CallHub_Async<TReturnType>(
                HubConnection hubConnection,
                string methodName,
                params object[] args ) {
        if( hubConnection.State == HubConnectionState.Disconnected ) {
            await hubConnection.StartAsync();
        }
        await IClientDataAccess.WaitForConnected_Async( hubConnection, TimeSpan.FromSeconds(10) );
        if( hubConnection.State != HubConnectionState.Connected ) {
            throw new InvalidOperationException( "Could not connect to hub: "+hubConnection.State );
        }

        if( args.Length == 0 ) {
            return await hubConnection.InvokeAsync<TReturnType>(
                methodName: methodName
            );
        } else if( args.Length == 1 ) {
            return await hubConnection.InvokeAsync<TReturnType>(
                methodName: methodName,
                arg1: args[0]
            );
        } else {
            throw new NotImplementedException( "CallHub does not yet support more than 1 argument" );
        }
    }
}
