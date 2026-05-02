using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Components.Application;
using MindCabinet.Client.Services;
using System.Text;

namespace MindCabinet.Client.Components.Layout;


public partial class MainPanel : ComponentBase {
    //[Inject]
    //public IJSRuntime Js { get; set; } = null!;

    [Inject]
    private ClientSessionManager SessionData { get; set; } = null!;

    //[Inject]
    //public LocalData LocalData { get; set; } = null!;


    [Parameter]
    public string? AddedClasses { get; set; } = null;


    [Parameter]
    public string? Source { get; set; } = null;


    private ContextPostsBrowser BrowserComponent = null!;



	protected override async Task OnInitializedAsync() {
		await base.OnInitializedAsync();

        await this.SessionData.RegisterUserAndAppDataEvent_Async( "MainPanel", async (_) => this.StateHasChanged() );
	}
}
