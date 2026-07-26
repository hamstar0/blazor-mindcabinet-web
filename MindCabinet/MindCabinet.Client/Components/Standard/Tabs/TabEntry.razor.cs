using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services;
using MindCabinet.Shared.Utility;


namespace MindCabinet.Client.Components.Standard.Tabs;


public partial class TabEntry : ComponentBase {
    [Inject]
    private TabNavigation Navigator { get; set; } = null!;


    [CascadingParameter(Name = "TabsContainer")]
    public Tabs TabsContainer { get; set; } = null!;

    [CascadingParameter]
    protected List<TabEntry> TabsRegistry { get; set; } = null!;

    
    [Parameter, EditorRequired]
    public string Id { get; set; } = null!;

    
    [Parameter, EditorRequired]
    public RenderFragment Header { get; set; }

    [Parameter, EditorRequired]
    public RenderFragment Content { get; set; }



    protected override void OnInitialized() {
        base.OnInitialized();

        List<string> parentRoute = this.TabsContainer.GetTabRoute();

        this.Navigator.RegisterTabRoute( parentRoute.Append(this.Id), this );

        this.TabsRegistry.Add( this );

        this.TabsContainer.Refresh_UI();
    }
}
