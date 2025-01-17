using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MindCabinet.Shared.DataEntries;


namespace MindCabinet.Client.Components.Application.Renders;


public partial class MultiTermRender : ComponentBase {
    //[Inject]
    //public IJSRuntime Js { get; set; } = null!;

    //[Inject]
    //public ClientDataAccess Data { get; set; } = null!;


    [Parameter]
    public string? AddedClasses { get; set; } = null;


    [Parameter, EditorRequired]
	public IEnumerable<TermEntry> Terms { get; set; } = null!;

	[Parameter]
	public Func<TermEntry, MouseEventArgs, Task>? OnClick_Async { get; set; } = null;

    public bool HasFocus { get; private set; } = false;
}