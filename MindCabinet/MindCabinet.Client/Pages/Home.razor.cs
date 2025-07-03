using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using MindCabinet.Client.Services;
using MindCabinet.Shared.DataEntries;
using System.Net.Http.Json;
using static MindCabinet.Client.Services.ClientSessionData;
using static System.Net.WebRequestMethods;

namespace MindCabinet.Client.Pages;



public partial class Home : ComponentBase {
    //[Inject]
    //public IJSRuntime Js { get; set; } = null!;

    //[Inject]
    //public IServiceProvider ServiceProvider { get; set; } = null!;

    [Inject]
    public HttpClient Http { get; set; } = null!;

    //[Inject]
    //public ClientDbAccess DbAccess { get; set; } = null!;

    //[Inject]
    //public ClientSessionData SessionData { get; set; } = null!;



    //protected async override Task OnParametersSetAsync() {
    //    await this.SessionData.Load_Async( this.Http );
    //}
}
