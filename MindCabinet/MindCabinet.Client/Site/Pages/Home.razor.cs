using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using MindCabinet.Client.Components.SiteFunctions;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.UserContext;
using MindCabinet.Shared.Utility;
using System.Net.Http.Json;

namespace MindCabinet.Client.Site.Pages;



public partial class Home : ComponentBase {
    private UserLoginForm? LoginFormElement = null;

    
    [Inject]
    private INetMode NetMode { get; set; } = null!;
    
    //[Inject]
    //public IJSRuntime Js { get; set; } = null!;

    [Inject]
    private HttpClient Http { get; set; } = null!;

    //[Inject]
    //public ClientDbAccess DbAccess { get; set; } = null!;

    [Inject]
    private ClientSessionData SessionData { get; set; } = null!;



    protected async override Task OnInitializedAsync() {
        await base.OnInitializedAsync();

        if( await this.SessionData.Load_Async() ) {
            this.StateHasChanged();
        }
    }
}
