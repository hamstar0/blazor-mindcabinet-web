using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsQuery;


namespace MindCabinet.Client.Components.Application.Renders;


public partial class MultiPostsQueryRender : ComponentBase {
    //[Inject]
    //public IJSRuntime Js { get; set; } = null!;


    [Parameter]
    public string? AddedClasses { get; set; } = null;
    

    [Parameter, EditorRequired]
	public IEnumerable<PostsQueryObject> Queries { get; set; } = null!;

	[Parameter]
	public Func<PostsQueryObject, MouseEventArgs, Task>? OnClick_Async { get; set; } = null;
    
    public bool HasFocus { get; private set; } = false;
}