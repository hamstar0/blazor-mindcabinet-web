using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using System.Net.Http.Json;

namespace MindCabinet.Client.Services;


public partial class ClientSessionData {
    public string? SessionId { get => this.ServerData?.SessionId; }

    public long? UserId { get => this.ServerData?.UserData?.Id; }

    public string? UserName { get => this.ServerData?.UserData?.Name; }



    public void LocalLogin( SimpleUserObject.ClientData user ) {
        if( this.ServerData is not null ) {
            this.ServerData.UserData = user;
        }
    }


    public const string Logout_Path = "Session";
    public const string Logout_Route = "Logout";

    public async Task Logout_Async( HttpClient httpClient ) {
        if( this.ServerData is null ) {
            return;
        }

        HttpResponseMessage msg = await httpClient.GetAsync(
            $"{Logout_Path}/{Logout_Route}"
        );

        msg.EnsureSuccessStatusCode();

        this.ServerData.UserData = null;
    }
}
