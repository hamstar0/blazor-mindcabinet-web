using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MindCabinet.Client.Services;
using MindCabinet.Shared.DataObjects.Term;


namespace MindCabinet.Client.Components.Application.Pickers;


public partial class TermPicker : ComponentBase {
    //[Inject]
    //public IJSRuntime Js { get; set; } = null!;

    [Inject]
    public ClientDbAccess DbAccess { get; set; } = null!;


    [Parameter]
    public string? AddedClasses { get; set; } = null;


    private bool IsSeachFocused = false;

    private string Value = "";

    private bool IsCurrentInputSuppressed = false;

    private List<TermObject> SearchOptions = new List<TermObject>();

    private int SearchPosition = -1;


    [Parameter]
    public bool Disabled { get; set; } = false;

    [Parameter, EditorRequired]
    public Func<TermObject, Task> OnTermSelect_Async { get; set; } = null!;


    private async Task HandleInput_Async( KeyboardEventArgs arg ) {
        int optionCount = this.SearchOptions.Count();
        if( optionCount == 0 ) {
            return;
        }

        bool isEnter = arg.Key == "Enter" || arg.Code == "NumpadEnter";

        switch( arg.Key ) {
        case "ArrowUp":
            this.IsCurrentInputSuppressed = optionCount > 0;
            this.SearchPosition--;
            break;
        case "ArrowDown":
            this.IsCurrentInputSuppressed = optionCount > 0;
            this.SearchPosition++;
            break;
        }
        if( isEnter ) {
            this.IsCurrentInputSuppressed = optionCount > 0;
        }

        this.SearchPosition = Math.Clamp( this.SearchPosition, 0, optionCount - 1 );

        if( isEnter && optionCount > 0 ) {
            await this.SelectSearchResults_Async( this.SearchOptions[this.SearchPosition] );
        }

        this.Value = this.SearchOptions[ this.SearchPosition ]?.Term ?? "";
    }


    private async Task SearchAndStoreTerms_Async( string termText ) {
        IEnumerable<TermObject> terms = await this.DbAccess.GetTermsByCriteria_Async(
            new ClientDbAccess.GetTermsByCriteriaParams( termText, null )
        );
        this.SearchOptions = terms.ToList();    // TODO
    }


    private async Task SelectSearchResults_Async( TermObject term ) {
        this.Value = term.Term ?? "";

Console.WriteLine( $"SelectSearchResults_Async: '{term.Term}'." );
        await this.OnTermSelect_Async.Invoke( term );

       // await this.Js.InvokeVoidAsync(
       //     "window.TermInputComponent.SetTermSearchResult",
       //     new object[] { this.InputElement, term.ToString() ?? "" }
       //);
    }
}
