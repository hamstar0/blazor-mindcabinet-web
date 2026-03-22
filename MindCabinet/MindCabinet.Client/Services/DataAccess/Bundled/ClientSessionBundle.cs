using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.UserContext;
using MindCabinet.Shared.Utility;
using System.Net.Http.Json;
using System.Text.Json;

namespace MindCabinet.Client.Services.DbAccess.Bundled;


public partial class ClientDataAccess_ClientSessionBundle : IClientDataAccess {
    public class GetCurrent_Return {
        public string SessionId { get; set; } = "";

        public SimpleUserObject.ClientObject? UserData { get; set; }

        public UserAppDataObject.Raw? UserAppData { get; set; }

        public UserContextObject.Raw? UserAppData_UserContext { get; set; }
    }

    public const string GetCurrent_Path = "Session";
    public const string GetCurrent_Route = "GetCurrent";

    public async Task<ClientSessionData.DataBundle> GetCurrent_Async(
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
        GetCurrent_Return? sessionData = await msg.Content.ReadFromJsonAsync<GetCurrent_Return>();
        if( sessionData is null ) {
            throw new InvalidDataException( "Could not deserialize ClientDataAccess_ClientSessionBundle.GetCurrent_Return" );
        }

        Task<UserAppDataObject>? userAppDataMaybeTask = sessionData.UserAppData_UserContext is not null
            ? sessionData.UserAppData?.CreateDataObject_Async(
                userContextFactory: async ( _ ) => await ClientDataAccess_UserContext.ToObject_Async( termsData, sessionData.UserAppData_UserContext! )
            )
            : null;
        UserAppDataObject? userAppDataMaybe = userAppDataMaybeTask is not null
            ? await userAppDataMaybeTask
            : null;

        return new ClientSessionData.DataBundle(
            sessionId: sessionData.SessionId,
            userData: sessionData.UserData,
            userAppData: userAppDataMaybe
        );
    }
}
