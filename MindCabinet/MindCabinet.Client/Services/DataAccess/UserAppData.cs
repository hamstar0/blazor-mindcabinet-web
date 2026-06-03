using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Threading;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects.PostsContext;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Components;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_UserAppData : IClientDataAccess {
    public static UserAppDataObject.Raw? Cache_ForCurrentUser { get; private set; } = null;



    private HubConnection HubConnection;



    public ClientDataAccess_UserAppData( NavigationManager navigationManager ) {
        Uri hubUrl = navigationManager.ToAbsoluteUri( IAPI.BaseRoute );
        this.HubConnection = new HubConnectionBuilder()
            .WithUrl( hubUrl )
            .Build();
    }

    public async ValueTask DisposeAsync() {
        await this.HubConnection.DisposeAsync();
    }

    public async Task<IAPI.GetForCurrentUser_Return> GetForCurrentUser_Async() {
        if( Cache_ForCurrentUser is not null ) {
            return new IAPI.GetForCurrentUser_Return { UserAppData = Cache_ForCurrentUser };
        }

        //

        var ret = await IClientDataAccess.CallHub_Async<IAPI.GetForCurrentUser_Return>(
            hubConnection: this.HubConnection,
            methodName: nameof( IAPI.GetForCurrentUser_Async ),
            args: new object[] { }
        );

        //

        Cache_ForCurrentUser = ret.UserAppData;

        //

        return ret;
    }


    public async Task UpdateForCurrentUser_Async( UserAppDataObject.Prototype parameters ) {
        await IClientDataAccess.CallHub_Async<object>(
            hubConnection: this.HubConnection,
            methodName: nameof( IAPI.UpdateForCurrentUser_Async ),
            args: new object[] { parameters }
        );

        //

        Cache_ForCurrentUser = null;
    }
}
