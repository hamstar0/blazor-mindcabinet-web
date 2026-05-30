using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.PostsContext;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.Utility;
using System.Net.Http.Json;
using System.Text.Json;

namespace MindCabinet.Client.Services.DbAccess.Bundled;


public partial class ClientDataAccess_ClientSessionBundle : IClientDataAccess {
    private HubConnection HubConnection;



    public ClientDataAccess_ClientSessionBundle( NavigationManager navigationManager ) {
        Uri hubUrl = navigationManager.ToAbsoluteUri( IAPI.BaseRoute );
        this.HubConnection = new HubConnectionBuilder()
            .WithUrl( hubUrl )
            .Build();
    }

    public async ValueTask DisposeAsync() {
        await this.HubConnection.DisposeAsync();
    }



    public async Task<LocalClientSessionManager.DataBundle> GetCurrent_Async(
                ClientDataAccess_Terms termsData ) {
        IAPI.GetCurrentDataBundle_Return? sessionData = await IClientDataAccess.CallHub_Async<IAPI.GetCurrentDataBundle_Return>(
            hubConnection: this.HubConnection,
            methodName: nameof( IAPI.GetCurrent_Async ),
            args: new object[] { new object() }
        );

        Task<UserAppDataObject>? userAppDataMaybeTask = null;
        if( sessionData.UserAppData_PostsContext is not null && sessionData.UserAppData_UserDefaultTerm is not null ) {
            userAppDataMaybeTask = sessionData.UserAppData?.ToDataObject_Async(
                termsFactory: async ( _ ) =>
                    await ClientDataAccess_Terms.ConvertRawToDataObject_Async( termsData, sessionData.UserAppData_UserDefaultTerm! ),
                postsContextFactory: async ( _ ) =>
                    await ClientDataAccess_PostsContext.ConvertRawToDataObject_Async( termsData, sessionData.UserAppData_PostsContext! )
            );
        }
        
        UserAppDataObject? userAppDataMaybe = userAppDataMaybeTask is not null
            ? await userAppDataMaybeTask
            : null;

        return new LocalClientSessionManager.DataBundle(
            sessionId: sessionData.SessionId,
            userData: sessionData.UserData,
            userAppData: userAppDataMaybe
        );
    }
}
