using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services;

namespace MindCabinet.Client.Components;



public partial class RenderPortalRegion : ComponentBase, IDisposable {
    [Inject]
    private RenderPortalService Portal { get; set; } = null!;

    [Parameter, EditorRequired]
    public string Region { get; set; } = "";

    [Parameter]
    public Action? OnChange { get; set; } = null;



    protected override void OnInitialized() {
        this.Portal.OnChange += this.OnChange ?? this.StateHasChanged;
    }

    public void Dispose() {
        this.Portal.OnChange -= this.OnChange ?? this.StateHasChanged;
    }
}
