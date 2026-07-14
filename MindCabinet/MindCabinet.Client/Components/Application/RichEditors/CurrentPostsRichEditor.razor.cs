using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Components.Application.Browsers;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DataPresenters;
using System.Text;

namespace MindCabinet.Client.Components.Application.RichEditors;


public partial class CurrentPostsRichEditor : ComponentBase {
    //[Inject]
    //public IJSRuntime Js { get; set; } = null!;

    [Inject]
    private LocalClientSessionManager MySessionMngr { get; set; } = null!;

    [Inject]
    private EachCurrentPostsSupplierSupplier TabsData { get; set; } = null!;


    [Parameter]
    public string? AddedClasses { get; set; } = null;


    private CurrentPostsBrowserTabs? TabbedPostsBrowserComponent = null!;



	protected override async Task OnInitializedAsync() {
        await base.OnInitializedAsync();

        await this.MySessionMngr.RegisterUserAndAppDataEvent_Async(
            name: nameof(CurrentPostsRichEditor),
            callback: async (_) => this.StateHasChanged()
        );
	}
}
