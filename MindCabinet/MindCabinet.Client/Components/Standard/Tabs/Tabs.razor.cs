using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services;
using System.Collections;
using System.ComponentModel;

namespace MindCabinet.Client.Components.Standard.Tabs;


public partial class Tabs : ComponentBase {
    [Inject]
    private TabNavigation Navigator { get; set; } = null!;


    [CascadingParameter(Name = "TabId")]
    private string? EnclosingTabId { get; set; }


    [Parameter, EditorRequired]
    public string Id { get; set; } = null!;

    
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



    public void ChangeTab( string tabId ) {
        int idx;
        for( idx=0; idx<this.TabsRegistry.Count; idx++ ) {
            if( this.TabsRegistry[idx].Id == tabId ) {
                break;
            }
        }

        if( idx >= this.TabsRegistry.Count ) {
            return;
        }

        this.ChangeTab( idx );
    }

    private void ChangeTab( int index ) {
        if( index < 0 || index >= this.TabsRegistry.Count ) {
            throw new ArgumentOutOfRangeException( nameof(index), $"Index {index} is out of range for TabsRegistry (count: {this.TabsRegistry.Count})" );
        }
        this.CurrentTabIndex = index;

        this.StateHasChanged();
    }


    public List<string> GetTabRoute() {
        List<string> route = [];

        Tabs currentTabContainer = this;
        string? parentTabId = this.EnclosingTabId;

        while( parentTabId is not null ) {
            TabEntry? enclosingTab = this.Navigator.GetRegisteredTabById( parentTabId );
            if( enclosingTab is null ) {
                throw new Exception( $"Tab {parentTabId} expected" );
            }

            route = enclosingTab.TabsContainer.GetTabRoute();
            route.Add( parentTabId );
            
            currentTabContainer = enclosingTab.TabsContainer;
            parentTabId = currentTabContainer.EnclosingTabId;
        }

        return route;
    }
}
