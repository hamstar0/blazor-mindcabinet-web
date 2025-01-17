using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MindCabinet.Shared.DataEntries;


namespace MindCabinet.Client.Components.Application.Renders;


public partial class MultiPostRender : ComponentBase {
    //[Inject]
    //public IJSRuntime Js { get; set; } = null!;

    //[Inject]
    //public ClientDataAccess Data { get; set; } = null!;


    [Parameter]
    public string? AddedClasses { get; set; } = null;


    [Parameter, EditorRequired]
    public IEnumerable<PostEntry> Posts { get; set; } = [];

    [Parameter]
	public Func<PostEntry, MouseEventArgs, Task>? OnClick_Async { get; set; } = null;
}