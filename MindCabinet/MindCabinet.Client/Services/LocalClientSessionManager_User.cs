using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using System.Net.Http.Json;

namespace MindCabinet.Client.Services;


public partial class LocalClientSessionManager {
    public string? SessionId => this.Data?.SessionId;

    public SimpleUserId? UserId => this.Data?.UserData?.Id;

    public string? UserName => this.Data?.UserData?.Name;



    public async Task LocalLogin_Async( SimpleUserObject.ClientObject user ) {
        if( this.Data is null ) {
            throw new InvalidOperationException( "UserAndAppData is null in LocalLogin." );
        }

        await this.LoadData_Async();
        
        this.Data.UserData = user;

        await this.TriggerUserLogin_Async( user );
    }


    public async Task Logout_Async( HttpClient httpClient ) {
        if( this.Data is null ) {
            return;
        }

        await IClientDataAccess.CallAPI_Async<int>(
            http: httpClient,
            route: $"{ClientDataAccess_UserSession.IAPI.BaseRoute}/{nameof(ClientDataAccess_UserSession.IAPI.Logout_Async)}",
            parameters: 0
        );

        SimpleUserObject.ClientObject? user = this.Data.UserData;

        await this.UnloadData_Async( false );
        await this.TriggerUserLogout_Async( user! );
    }
}
