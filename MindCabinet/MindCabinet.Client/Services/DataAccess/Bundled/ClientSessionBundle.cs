using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.PostsContext;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.Utility;
using System.Net.Http.Json;
using System.Text.Json;

namespace MindCabinet.Client.Services.DbAccess.Bundled;


public partial class ClientDataAccess_ClientSessionBundle : IClientDataAccess {
    private HttpClient Http;



    public ClientDataAccess_ClientSessionBundle( HttpClient http ) {
        this.Http = http;
    }



    public async Task<LocalClientSessionManager.DataBundle> GetCurrent_Async( ClientDataAccess_Terms termsDataSrc ) {
        var sessionData = await IClientDataAccess.CallAPI_Async<IAPI.GetCurrentDataBundle_Return>(
            http: this.Http,
            route: $"{IAPI.BaseRoute}/{nameof(IAPI.GetCurrent_Async)}"
        );

        Task<UserAppDataObject>? userAppDataMaybeTask = null;
        if( sessionData.UserAppData_PostsContext is not null && sessionData.UserAppData_UserDefaultTerm is not null ) {
            userAppDataMaybeTask = sessionData.UserAppData?.ToDataObject_Async(
                termsFactory: async ( _ ) =>
                    await ClientDataAccess_Terms.ConvertRawToDataObject_Async( termsDataSrc, sessionData.UserAppData_UserDefaultTerm! ),
                postsContextFactory: async ( _ ) =>
                    await ClientDataAccess_PostsContext.ConvertRawToDataObject_Async( termsDataSrc, sessionData.UserAppData_PostsContext! )
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
