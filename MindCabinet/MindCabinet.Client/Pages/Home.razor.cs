using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.UserContext;
using System.Net.Http.Json;
using static MindCabinet.Client.Services.ClientSessionData;
using static System.Net.WebRequestMethods;

namespace MindCabinet.Client.Pages;



public partial class Home : ComponentBase {
    //[Inject]
    //private IJSRuntime Js { get; set; } = null!;

    //[Inject]
    //private IServiceProvider ServiceProvider { get; set; } = null!;

    [Inject]
    private HttpClient Http { get; set; } = null!;

    //[Inject]
    //private ClientDbAccess DbAccess { get; set; } = null!;;



    //protected async override Task OnParametersSetAsync() {
    //    await this.SessionData.Load_Async( this.Http );
    //}
}
