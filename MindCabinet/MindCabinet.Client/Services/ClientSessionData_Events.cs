using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserContext;
using System.Net.Http.Json;

namespace MindCabinet.Client.Services;


public partial class ClientSessionData {
    private Dictionary<string, Func<DataBundle, Task>> OnUserAndAppDataLoaded_Async = new();
    private DataBundle? OnUserAndAppDataLoaded_PromisedData = null;

    private Dictionary<string, Func<UserContextObject, Task>> OnUserContextChanged_Async = new();
    private UserContextObject? OnUserContextChanged_PromisedData = null;

    private Dictionary<string, Func<SimpleUserObject.ClientObject, Task>> OnUserLogin_Async = new();
    private SimpleUserObject.ClientObject? OnUserLogin_PromisedData = null;

    private Dictionary<string, Func<SimpleUserObject.ClientObject, Task>> OnUserLogout_Async = new();
    private SimpleUserObject.ClientObject? OnUserLogout_PromisedData = null;



    private async Task TriggerUserAndAppDataLoaded_Async( DataBundle data ) {
        this.OnUserAndAppDataLoaded_PromisedData = data;

        await Task.WhenAll(
            this.OnUserAndAppDataLoaded_Async
                .Select( kv => kv.Value.Invoke(data) )
        );
    }

    private async Task TriggerUserContextChanged_Async( UserContextObject context ) {
        this.OnUserContextChanged_PromisedData = context;

        await Task.WhenAll(
            this.OnUserContextChanged_Async
                .Select( kv => kv.Value.Invoke(context) )
        );
    }

    private async Task TriggerUserLogin_Async( SimpleUserObject.ClientObject user ) {
        this.OnUserLogin_PromisedData = user;

        await Task.WhenAll(
            this.OnUserLogin_Async
                .Select( kv => kv.Value.Invoke(user) )
        );
    }

    private async Task TriggerUserLogout_Async( SimpleUserObject.ClientObject user ) {
        this.OnUserLogout_PromisedData = user;

        await Task.WhenAll(
            this.OnUserLogout_Async
                .Select( kv => kv.Value.Invoke(user) )
        );
    }


    public async Task RegisterUserAndAppDataEvent_Async( string name, Func<DataBundle, Task> callback ) {
        this.OnUserAndAppDataLoaded_Async.Add( name, callback );

        if( this.OnUserAndAppDataLoaded_PromisedData is not null ) {
            await callback.Invoke( this.OnUserAndAppDataLoaded_PromisedData );
        }
    }

    public async Task RegisterUserContextEvent_Async( string name, Func<UserContextObject, Task> callback ) {
        this.OnUserContextChanged_Async.Add( name, callback );

        if( this.OnUserContextChanged_PromisedData is not null ) {
            await callback.Invoke( this.OnUserContextChanged_PromisedData );
        }
    }

    public async Task RegisterUserLoginEvent_Async( string name, Func<SimpleUserObject.ClientObject, Task> callback ) {
        this.OnUserLogin_Async.Add( name, callback );

        if( this.OnUserLogin_PromisedData is not null ) {
            await callback.Invoke( this.OnUserLogin_PromisedData );
        }
    }

    public async Task RegisterUserLogoutEvent_Async( string name, Func<SimpleUserObject.ClientObject, Task> callback ) {
        this.OnUserLogout_Async.Add( name, callback );

        if( this.OnUserLogout_PromisedData is not null ) {
            await callback.Invoke( this.OnUserLogout_PromisedData );
        }
    }
}
