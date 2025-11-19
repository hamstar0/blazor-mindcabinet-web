using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MindCabinet.Shared.DataObjects;


namespace MindCabinet.Client.Components.Application.Renders;


public partial class SimplePostRender : ComponentBase {
    //[Inject]
    //public IJSRuntime Js { get; set; } = null!;

    //[Inject]
    //public ClientDataAccess Data { get; set; } = null!;


    [Parameter]
    public string? AddedClasses { get; set; } = null;


    [Parameter, EditorRequired]
	public SimplePostObject Post { get; set; } = null!;

	[Parameter]
	public Func<MouseEventArgs, Task>? OnClick_Async { get; set; } = null;
}