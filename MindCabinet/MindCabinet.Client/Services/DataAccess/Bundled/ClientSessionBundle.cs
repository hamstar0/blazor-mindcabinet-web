using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.PostsContext;
using MindCabinet.Shared.Utility;
using System.Net.Http.Json;
using System.Text.Json;

namespace MindCabinet.Client.Services.DbAccess.Bundled;


public partial class ClientDataAccess_ClientSessionBundle : IClientDataAccess {
    public class GetCurrentDataBundle_Return {
        public string SessionId { get; set; } = "";

        public SimpleUserObject.ClientObject? UserData { get; set; }

        public UserAppDataObject.Raw? UserAppData { get; set; }

        public PostsContextObject.Raw? UserAppData_PostsContext { get; set; }
    }

    public const string GetCurrent_Path = "Session";
    public const string GetCurrent_Route = "GetCurrentDataBundle";

    public async Task<ClientSessionManager.DataBundle> GetCurrent_Async(
                HttpClient httpClient,
                ClientDataAccess_Terms termsData ) {
        HttpResponseMessage msg = await httpClient.PostAsJsonAsync(
            $"{GetCurrent_Path}/{GetCurrent_Route}",
            new object()
        );

        msg.EnsureSuccessStatusCode();

        // string rawData = await msg.Content.ReadAsStringAsync();
        // if( string.IsNullOrWhiteSpace(rawData) || rawData == "{}" ) {
        //     throw new InvalidDataException( "Did not receive JSON string for GetCurrent_Return" );
        // }

        // var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        // GetCurrent_Return? sessionData = JsonSerializer.Deserialize<GetCurrent_Return>(
        //     json: rawData,
        //     options: options
        // );
        GetCurrentDataBundle_Return? sessionData = await msg.Content.ReadFromJsonAsync<GetCurrentDataBundle_Return>();
        if( sessionData is null ) {
            throw new InvalidDataException( "Could not deserialize ClientDataAccess_ClientSessionBundle.GetCurrent_Return" );
        }

        Task<UserAppDataObject>? userAppDataMaybeTask = sessionData.UserAppData_PostsContext is not null
            ? sessionData.UserAppData?.ToDataObject_Async(
                postsContextFactory: async ( _ ) =>
                    await ClientDataAccess_PostsContext.ConvertRawToDataObject_Async( termsData, sessionData.UserAppData_PostsContext! )
            )
            : null;
        UserAppDataObject? userAppDataMaybe = userAppDataMaybeTask is not null
            ? await userAppDataMaybeTask
            : null;

        return new ClientSessionManager.DataBundle(
            sessionId: sessionData.SessionId,
            userData: sessionData.UserData,
            userAppData: userAppDataMaybe
        );
    }
}
