using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Components.Application;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DataPresenters;
using System.Text;

namespace MindCabinet.Client.Components.Layout;


public partial class MainPanel : ComponentBase {
    //[Inject]
    //public IJSRuntime Js { get; set; } = null!;

    [Inject]
    private LocalClientSessionManager MySessionMngr { get; set; } = null!;


    [Parameter]
    public string? AddedClasses { get; set; } = null;



    protected async override Task OnInitializedAsync() {
        await base.OnInitializedAsync();
    }
}
