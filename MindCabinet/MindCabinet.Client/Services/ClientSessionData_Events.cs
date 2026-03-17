using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserContext;
using System.Net.Http.Json;

namespace MindCabinet.Client.Services;


public partial class ClientSessionData {
    private List<Func<DataBundle, Task>> OnUserAndAppDataLoaded_Async = new();

    private List<Func<UserContextObject, Task>> OnUserContextChanged_Async = new();

    private List<Func<SimpleUserObject.ClientObject, Task>> OnUserLogin_Async = new();

    private List<Func<SimpleUserObject.ClientObject, Task>> OnUserLogout_Async = new();



    public void RegisterUserAndAppDataEvent( Func<DataBundle, Task> callback ) {
        this.OnUserAndAppDataLoaded_Async.Add( callback );
    }

    public void RegisterUserContextEvent( Func<UserContextObject, Task> callback ) {
        this.OnUserContextChanged_Async.Add( callback );
    }

    public void RegisterUserLoginEvent( Func<SimpleUserObject.ClientObject, Task> callback ) {
        this.OnUserLogin_Async.Add( callback );
    }

    public void RegisterUserLogoutEvent( Func<SimpleUserObject.ClientObject, Task> callback ) {
        this.OnUserLogout_Async.Add( callback );
    }
}
