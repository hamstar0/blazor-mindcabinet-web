using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserContext;


namespace MindCabinet.Client.Components.Application.Renders;


public partial class UserContextRender : ComponentBase {
    //[Inject]
    //public IJSRuntime Js { get; set; } = null!;

    [Inject]
    private ClientDataAccess_UserContext UserContextsData { get; set; } = null!;


    [Parameter]
    public string? AddedClasses { get; set; } = null;


    [Parameter]
    public RenderFragment? ChildContent { get; set; } = null;


    [Parameter, EditorRequired]
	public UserContextObject Context { get; set; } = null!;

	[Parameter]
	public Func<MouseEventArgs, Task>? OnClick_Async { get; set; } = null;
}