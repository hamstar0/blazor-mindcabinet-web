using Microsoft.AspNetCore.Components;

namespace MindCabinet.Client.Components.Standard;


public partial class Pagination : ComponentBase {
    //[Inject]
    //public IJSRuntime Js { get; set; } = null!;

    //[Inject]
    //public ClientDataAccess Data { get; set; } = null!;


    [Parameter]
    public string? AddedClasses { get; set; } = null;


    [Parameter, EditorRequired]
    public int CurrentPage { get; set; }

    [Parameter, EditorRequired]
    public int TotalPages { get; set; }
    
    [Parameter]
    public int MaxPagesToDisplay { get; set; } = 20;

    [Parameter, EditorRequired]
    public Func<int, Task> OnPageChange_Async { get; set; } = null!;
}
