using Microsoft.AspNetCore.Components;
using System.Collections;

namespace MindCabinet.Client.Components.Standard.Tabs;


public partial class Tabs : ComponentBase {
    [Parameter]
    public int InitialTabIndex { get; set; } = 0;

    public int CurrentTabIndex { get; private set; }


    private List<TabEntry> TabsRegistry = new List<TabEntry>();

    private TabEntry? CurrentTab => this.CurrentTabIndex >= this.TabsRegistry.Count
        ? null
        : this.TabsRegistry[ this.CurrentTabIndex ];



	protected override void OnInitialized() {
		base.OnInitialized();

        this.CurrentTabIndex = this.InitialTabIndex;
	}



    public void ChangeTab( int index ) {
        if( index < 0 || index >= this.TabsRegistry.Count ) {
            throw new ArgumentOutOfRangeException( nameof(index), $"Index {index} is out of range for TabsRegistry (count: {this.TabsRegistry.Count})" );
        }
        this.CurrentTabIndex = index;

        this.StateHasChanged();
    }
}
