using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Components.Application;
using System.Text;

namespace MindCabinet.Client.Components.Layout;


public partial class Main : ComponentBase {
    //[Inject]
    //public IJSRuntime Js { get; set; } = null!;

    //[Inject]
    //public ClientDataAccess Data { get; set; } = null!;

    //[Inject]
    //public LocalData LocalData { get; set; } = null!;


    [Parameter]
    public string? AddedClasses { get; set; } = null;


    [Parameter]
    public string? Source { get; set; } = null;


    private SimplePostsBrowser BrowserComponent = null!;
}
