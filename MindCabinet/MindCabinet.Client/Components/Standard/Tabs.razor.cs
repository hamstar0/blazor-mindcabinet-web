using Microsoft.AspNetCore.Components;
using System.Collections;

namespace MindCabinet.Client.Components.Standard;


public partial class Tabs : ComponentBase {
    [Parameter]
    public int CurrentTabIndex { get; set; } = 0;

    public List<(RenderFragment Header, RenderFragment Content)> TabEntries = [];
}
