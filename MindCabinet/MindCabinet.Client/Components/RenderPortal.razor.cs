using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services;

namespace MindCabinet.Client.Components;


public partial class RenderPortal : ComponentBase, IDisposable {
    [Inject]
    private RenderPortalService Portal { get; set; } = null!;


    [Parameter, EditorRequired]
    public string TargetRegion { get; set; } = "";

    [Parameter, EditorRequired]
    public RenderFragment? ChildContent { get; set; } = null!;

    private Guid? Id;
    

    
    protected override void OnInitialized() {
        this.Id = this.Portal.Register( this.TargetRegion, this.ChildContent! );
    }

    public void Dispose() {
        if( this.Id.HasValue ) {
            this.Portal.Unregister( this.TargetRegion, this.Id.Value );
        }
    }
}
