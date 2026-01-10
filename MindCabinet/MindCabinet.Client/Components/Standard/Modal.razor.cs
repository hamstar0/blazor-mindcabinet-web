using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections;

namespace MindCabinet.Client.Components.Standard;


public partial class Modal : ComponentBase {
    [Parameter, EditorRequired]
    public string Title { get; set; } = null!;

    [Parameter, EditorRequired]
    public RenderFragment? ChildContent { get; set; } = null!;
    
    [Parameter]
    public string? AddedClasses { get; set; } = null;
    
    [Parameter]
    public string? AddedStyles { get; set; } = null;


    public string FullStyles => $"{this.ModalDisplay} {this.AddedStyles}";


    public bool IsOpen => this.ModalClass == "Show";


    //private Guid Guid = Guid.NewGuid();
    private string ModalDisplay = "display: none;";
    private string ModalClass = "";
    private bool ShowBackdrop = false;



    public void Open() {
        this.ModalDisplay = "display: block;";
        this.ModalClass = "Show";
        this.ShowBackdrop = true;
        this.StateHasChanged();
    }

    public void Close() {
        this.ModalDisplay = "display: none";
        this.ModalClass = "";
        this.ShowBackdrop = false;
        this.StateHasChanged();
    }
}
