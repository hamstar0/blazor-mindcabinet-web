using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Components.Application;
using MindCabinet.Client.Services;
using System.Text;

namespace MindCabinet.Client.Components.Layout;


public partial class MainPanel : ComponentBase {
    //[Inject]
    //public IJSRuntime Js { get; set; } = null!;

    [Inject]
    private LocalClientSessionManager MySessionMngr { get; set; } = null!;


    [Parameter]
    public string? AddedClasses { get; set; } = null;


    [Parameter]
    public string? Source { get; set; } = null;


    private PostsBrowser? BrowserComponent = null!;



	protected override async Task OnInitializedAsync() {
		await base.OnInitializedAsync();

        await this.MySessionMngr.RegisterUserAndAppDataEvent_Async(
            name: "MainPanel",
            callback: async (_) => this.StateHasChanged()
        );

        await this.MySessionMngr.RegisterPostsContextEvent_Async(
            name: "MainPanel",
            callback: async (_) => {
                if( this.BrowserComponent is not null ) {
                    await this.BrowserComponent.RefreshPosts_Async();
                }
            }
        );
	}
}
