using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;


namespace MindCabinet.Client.Components.Standard;


public partial class Tooltip : ComponentBase {
    private class PositionResult {
        public double X { get; set; }
        public double Y { get; set; }
    }
    


    [Inject]
    public IJSRuntime Js { get; set; } = null!;
    
    [Parameter]
    public RenderFragment Anchor { get; set; } = null!;
    
    [Parameter]
    public RenderFragment TooltipContent { get; set; } = null!;

    [Parameter]
    public Func<bool>? Enabled { get; set; }
    

    private bool IsVisible { get; set; }

    private double X { get; set; }

    private double Y { get; set; }


    private ElementReference TooltipElement;




    private async Task ShowTooltip_Async( MouseEventArgs e ) {
        if( this.Enabled?.Invoke() == false ) {
            return;
        }

        PositionResult position = await this.Js.InvokeAsync<PositionResult>(
            identifier: "GetTooltipPosition",
            this.TooltipElement,
            e.ClientX,
            e.ClientY
        );

        this.X = position.X;
        this.Y = position.Y;
        this.IsVisible = true;
    }

    private void HideTooltip() {
        this.IsVisible = false;
    }
}

