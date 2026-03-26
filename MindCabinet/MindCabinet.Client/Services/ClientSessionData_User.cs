using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using System.Net.Http.Json;

namespace MindCabinet.Client.Services;


public partial class ClientSessionData {
    public string? SessionId => this.Data?.SessionId;

    public SimpleUserId? UserId => this.Data?.UserData?.Id;

    public string? UserName => this.Data?.UserData?.Name;



    public async Task LocalLogin_Async( SimpleUserObject.ClientObject user ) {
        if( this.Data is null ) {
            throw new InvalidOperationException( "UserAndAppData is null in LocalLogin." );
        }
        
        this.Data.UserData = user;

        await this.TriggerUserLogin_Async( user );
    }


    public const string Logout_Path = "Session";
    public const string Logout_Route = "Logout";

    public async Task Logout_Async( HttpClient httpClient ) {
        if( this.Data is null ) {
            return;
        }

        HttpResponseMessage msg = await httpClient.GetAsync(
            $"{Logout_Path}/{Logout_Route}"
        );

        msg.EnsureSuccessStatusCode();

        await this.TriggerUserLogout_Async( this.Data.UserData! );

        this.Data.UserData = null;
    }
}
