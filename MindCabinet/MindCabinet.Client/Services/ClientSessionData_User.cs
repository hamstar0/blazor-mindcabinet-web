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



    public void Login( SimpleUserObject.ClientData user ) {
        if( this.ServerData is not null ) {
            this.ServerData.UserData = user;
        }
    }

    public void Logout() {
        if( this.ServerData is not null ) {
            this.ServerData.UserData = null;
        }
    }
}
