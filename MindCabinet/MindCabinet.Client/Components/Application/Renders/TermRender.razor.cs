using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MindCabinet.Shared.DataEntries;


namespace MindCabinet.Client.Components.Application.Renders;


public partial class TermRender : ComponentBase {
    //[Inject]
    //public IJSRuntime Js { get; set; } = null!;

    //[Inject]
    //public ClientDataAccess Data { get; set; } = null!;


    [Parameter]
    public string? AddedClasses { get; set; } = null;

    [Parameter]
    public RenderFragment? ChildContent { get; set; } = null;


    [Parameter, EditorRequired]
	public TermEntry Term { get; set; } = null!;

	[Parameter]
	public Func<MouseEventArgs, Task>? OnClick_Async { get; set; } = null;
}