using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Threading;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using Microsoft.AspNetCore.SignalR.Client;


namespace MindCabinet.Client.Services.DataAccess;



public interface IClientDataAccess : IAsyncDisposable {
    public async static Task<TReturnType> CallHub_Async<TReturnType>(
                HubConnection hubConnection,
                string methodName,
                params object[] args ) {
        if( hubConnection.State != HubConnectionState.Connected ) {
            await hubConnection.StartAsync();
        }
        if( hubConnection.State != HubConnectionState.Connected ) {
            throw new InvalidOperationException( "Could not connect to hub" );
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
