using Microsoft.AspNetCore.Components;
using System.Collections;

namespace MindCabinet.Client.Components.Standard;


public partial class Tabs : ComponentBase {
    [Parameter]
    public int InitialTabIndex { get; set; } = 0;

    public int CurrentTabIndex { get; private set; }


    [Parameter]
    public IEnumerable<(RenderFragment Header, RenderFragment Content)> InitialTabEntries { get; set; } = [];

    private List<(RenderFragment Header, RenderFragment Content)> TabEntries = null!;



	protected override void OnInitialized() {
		base.OnInitialized();

        this.CurrentTabIndex = this.InitialTabIndex;
        this.TabEntries = this.InitialTabEntries.ToList();
	}


    public void InsertTab( RenderFragment header, RenderFragment content, int idx = -1 ) {
        if( idx == -1 ) {
            this.TabEntries.Add( (header, content) );
        } else if( idx >= 0 && idx <= this.TabEntries.Count ) {
            this.TabEntries.Insert( idx, (header, content) );
        } else {
            throw new ArgumentOutOfRangeException( nameof(idx), "Index must be between 0 and the number of tabs." );
        }
    }
}
