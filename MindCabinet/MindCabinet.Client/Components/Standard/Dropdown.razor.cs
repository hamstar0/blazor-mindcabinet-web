using Microsoft.AspNetCore.Components;
using System.Collections;

namespace MindCabinet.Client.Components.Standard;


public partial class Dropdown : ComponentBase {   // Dropdown<TKey> where TKey : struct, Enum
    [Parameter, EditorRequired]
    public IDictionary<int, string> Options { get; set; } = null!;


    [Parameter, EditorRequired]
    public Action<int> OnSelect { get; set; } = null!;


    [Parameter]
    public string? AddedClasses { get; set; } = null;

    [Parameter]
    public string? AddedStyle { get; set; } = null;


    private int CurrentSelection = 0;



    private void Select_UI( int option ) {
        this.CurrentSelection = option;

        this.OnSelect( option );
    }
}
