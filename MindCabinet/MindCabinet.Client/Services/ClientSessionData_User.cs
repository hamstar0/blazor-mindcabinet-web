using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using System.Net.Http.Json;

namespace MindCabinet.Client.Services;


public partial class ClientSessionData {
    public string? SessionId => this.Data?.SessionId;

    public long? UserId => this.Data?.UserData?.Id;

    public string? UserName => this.Data?.UserData?.Name;



    public async Task LocalLogin_Async( SimpleUserObject.ClientObject user ) {
        if( this.Data is null ) {
            throw new InvalidOperationException( "UserAndAppData is null in LocalLogin." );
        }
        
        this.Data.UserData = user;

        await Task.WhenAll(
            this.OnUserLogin_Async
                .Select( f => f.Invoke(user) )
        );
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

        await Task.WhenAll(
            this.OnUserLogout_Async
                .Select( f => f.Invoke(this.Data.UserData!) )
        );

        this.Data.UserData = null;
    }
}
