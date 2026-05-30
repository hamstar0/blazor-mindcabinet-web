using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.Utility;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_Terms : IClientDataAccess {
    private HubConnection HubConnection;



    public ClientDataAccess_Terms( NavigationManager navigationManager ) {
        Uri hubUrl = navigationManager.ToAbsoluteUri( IAPI.BaseRoute );
        this.HubConnection = new HubConnectionBuilder()
            .WithUrl( hubUrl )
            .Build();
    }

    public async ValueTask DisposeAsync() {
        await this.HubConnection.DisposeAsync();
    }


    public async Task<IAPI.GetByX_Return> GetByIds_Async( IEnumerable<TermId> termIds ) {
        return await IClientDataAccess.CallHub_Async<IAPI.GetByX_Return>(
            hubConnection: this.HubConnection,
            methodName: nameof( IAPI.GetByIds_Async ),
            args: new object[] { termIds }
        );
    }


    public async Task<IAPI.GetByX_Return> GetByCriteria_Async( IAPI.GetByCriteria_Params parameters ) {
        return await IClientDataAccess.CallHub_Async<IAPI.GetByX_Return>(
            hubConnection: this.HubConnection,
            methodName: nameof( IAPI.GetByCriteria_Async ),
            args: new object[] { parameters }
        );
    }


    public async Task<IAPI.Create_Return> Create_Async( IAPI.Create_Params parameters ) {
        return await IClientDataAccess.CallHub_Async<IAPI.Create_Return>(
            hubConnection: this.HubConnection,
            methodName: nameof( IAPI.Create_Async ),
            args: new object[] { parameters }
        );
    }
}
