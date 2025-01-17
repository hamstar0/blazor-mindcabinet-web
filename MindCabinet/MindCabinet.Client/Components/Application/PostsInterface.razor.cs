using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Data;
using MindCabinet.Shared.DataEntries;

namespace MindCabinet.Client.Components.Application;


public partial class PostsInterface : ComponentBase {
    //[Inject]
    //public IJSRuntime Js { get; set; } = null!;

    //[Inject]
    //public ClientDataAccess Data { get; set; } = null!;

    //[Inject]
    //public LocalData LocalData { get; set; } = null!;


    [Parameter]
    public string? AddedClasses { get; set; } = null;


    private PostsBrowser BrowserComponent = null!;
}