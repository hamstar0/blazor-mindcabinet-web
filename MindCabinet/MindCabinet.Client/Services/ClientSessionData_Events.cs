using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserPostsContext;
using System.Net.Http.Json;

namespace MindCabinet.Client.Services;


public partial class ClientSessionData {
    private Dictionary<string, Func<DataBundle?, Task>> OnUserAndAppDataLoaded_Async = new();
    private DataBundle? OnUserAndAppDataLoaded_PromisedData = null;

    private Dictionary<string, Func<UserPostsContextObject?, Task>> OnUserPostsContextChanged_Async = new();
    private UserPostsContextObject? OnUserPostsContextChanged_PromisedData = null;

    private Dictionary<string, Func<SimpleUserObject.ClientObject, Task>> OnUserLogin_Async = new();
    private SimpleUserObject.ClientObject? OnUserLogin_PromisedData = null;

    private Dictionary<string, Func<SimpleUserObject.ClientObject, Task>> OnUserLogout_Async = new();
    private SimpleUserObject.ClientObject? OnUserLogout_PromisedData = null;



    private async Task TriggerUserAndAppDataLoaded_Async( DataBundle? data ) {
        this.OnUserAndAppDataLoaded_PromisedData = data;

        await Task.WhenAll(
            this.OnUserAndAppDataLoaded_Async
                .Select( kv => kv.Value.Invoke(data) )
        );
    }

    private async Task TriggerUserPostsContextChanged_Async( UserPostsContextObject? context ) {
        this.OnUserPostsContextChanged_PromisedData = context;

        await Task.WhenAll(
            this.OnUserPostsContextChanged_Async
                .Select( kv => kv.Value.Invoke(context) )
        );
    }

    private async Task TriggerUserLogin_Async( SimpleUserObject.ClientObject user ) {
        this.OnUserLogin_PromisedData = user;

        await Task.WhenAll(
            this.OnUserLogin_Async
                .Select( kv => kv.Value.Invoke(user) )
        );

        if( this.Data is null ) {
            throw new InvalidOperationException( "Current session UserAndAppData is null in TriggerUserLogin." );
        }
        if( this.Data.UserAppData?.UserPostsContext is null ) {
            throw new InvalidOperationException( "Current session UserPostsContext is null in TriggerUserLogin." );
        }

        await this.TriggerUserAndAppDataLoaded_Async( this.Data );
        await this.TriggerUserPostsContextChanged_Async( this.Data.UserAppData?.UserPostsContext! );
    }

    private async Task TriggerUserLogout_Async( SimpleUserObject.ClientObject user ) {
        this.OnUserLogout_PromisedData = user;

        await Task.WhenAll(
            this.OnUserLogout_Async
                .Select( kv => kv.Value.Invoke(user) )
        );

        await this.TriggerUserAndAppDataLoaded_Async( null );
        await this.TriggerUserPostsContextChanged_Async( null );
    }


    public async Task RegisterUserAndAppDataEvent_Async( string name, Func<DataBundle?, Task> callback ) {
        this.OnUserAndAppDataLoaded_Async.Add( name, callback );

        if( this.OnUserAndAppDataLoaded_PromisedData is not null ) {
            await callback.Invoke( this.OnUserAndAppDataLoaded_PromisedData );
        }
    }

    public async Task RegisterUserPostsContextEvent_Async( string name, Func<UserPostsContextObject?, Task> callback ) {
        this.OnUserPostsContextChanged_Async.Add( name, callback );

        if( this.OnUserPostsContextChanged_PromisedData is not null ) {
            await callback.Invoke( this.OnUserPostsContextChanged_PromisedData );
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
