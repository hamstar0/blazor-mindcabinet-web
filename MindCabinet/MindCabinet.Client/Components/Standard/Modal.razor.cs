using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Collections;

namespace MindCabinet.Client.Components.Standard;


public partial class Modal : ComponentBase {
    [Inject]
    public IJSRuntime Js { get; set; } = null!;

    [Parameter, EditorRequired]
    public string ModalId { get; set; } = null!;

    [Parameter]
    public string Title { get; set; } = null!;

    [Parameter]
    public RenderFragment? HeaderContent { get; set; } = null!;

    [Parameter]
    public RenderFragment? BodyContent { get; set; } = null!;

    [Parameter]
    public RenderFragment? FooterContent { get; set; } = null!;
    
    [Parameter]
    public string? AddedClasses { get; set; } = null;
    
    [Parameter]
    public string? AddedStyles { get; set; } = null;


    public string FullStyles => $"{this.ModalDisplay} {this.AddedStyles}";


    public bool IsOpen => this.ModalClass == "Show";


    //private Guid Guid = Guid.NewGuid();
    private string ModalDisplay = "display: none;";
    private string ModalClass = "";
    // private bool ShowBackdrop = false;



    public RenderFragment GenerateDialogOpener( string buttonLabel ) {
        return builder => {
            int seq = 0;
            builder.OpenElement( seq++, "button" );
            builder.AddAttribute( seq++, "class", "btn btn-primary" );
            builder.AddAttribute( seq++, "type", "button" );
            builder.AddAttribute( seq++, "data-bs-toggle", "modal" );
            builder.AddAttribute( seq++, "data-bs-target", $"#{this.ModalId}" );
            builder.AddContent( seq++, buttonLabel );
            builder.CloseElement();
        };
    }


    public void Open() {
        // this.ModalDisplay = "display: block;";
        // this.ModalClass = "Show";
        // this.ShowBackdrop = true;

        this.Js.InvokeVoidAsync( "bootstrapOpenModal", this.ModalId );

        // this.StateHasChanged();
    }

    public void Close() {
        // this.ModalDisplay = "display: none";
        // this.ModalClass = "";
        // this.ShowBackdrop = false;

        this.Js.InvokeVoidAsync( "bootstrapCloseModal", this.ModalId );
        
        // this.StateHasChanged();
    }
}
