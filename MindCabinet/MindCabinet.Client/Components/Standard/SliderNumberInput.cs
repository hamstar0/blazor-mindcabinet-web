using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections;

namespace MindCabinet.Client.Components.Standard;


public partial class SliderNumberInput : ComponentBase {
    [Parameter]
    public string? AddedClasses { get; set; } = null;
    
    [Parameter]
    public string? AddedStyles { get; set; } = null;

    
    [Parameter]
    public double InitialValue { get; set; } = 0;

    [Parameter]
    public double Min { get; set; } = 0;

    [Parameter]
    public double Max { get; set; } = 100;

    [Parameter, EditorRequired]
    public Func<double, Task> OnValueChanged_Async { get; set; } = null!;
    

    private double Value;


	protected override void OnParametersSet() {
        this.Value = this.InitialValue;
	}
}
