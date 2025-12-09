using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserContext;


namespace MindCabinet.Client.Components.Application.Renders;


public partial class MultiUserContextRender : ComponentBase {
    //[Inject]
    //public IJSRuntime Js { get; set; } = null!;

    //[Inject]
    //public ClientDataAccess Data { get; set; } = null!;


    [Parameter]
    public string? AddedClasses { get; set; } = null;
    

    [Parameter, EditorRequired]
	public IEnumerable<UserContextObject> Contexts { get; set; } = null!;

	[Parameter]
	public Func<UserContextObject, MouseEventArgs, Task>? OnClick_Async { get; set; } = null;
    
    public bool HasFocus { get; private set; } = false;
}