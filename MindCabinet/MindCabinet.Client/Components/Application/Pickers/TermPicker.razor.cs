using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MindCabinet.Client.Services;
using MindCabinet.Shared.DataEntries;


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

    private IList<TermEntry> SearchOptions = new List<TermEntry>();

    private int SearchPosition = -1;


    [Parameter]
    public bool Disabled { get; set; } = false;

    [Parameter, EditorRequired]
    public Func<TermEntry, Task> OnTermSelect_Async { get; set; } = null!;



    private async Task OnInputKey_UI_Async( KeyboardEventArgs arg ) {
        if( this.Disabled ) {
            return;
        }
        if( !this.IsSeachFocused ) {
            return;
        }

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

        if( this.SearchPosition < 0 ) {
            this.SearchPosition = 0;
        } else if( this.SearchPosition >= optionCount ) {
            this.SearchPosition = optionCount - 1;
        }

        if( isEnter && optionCount > 0 ) {
            await this.SelectSearchResults_Async( this.SearchOptions[this.SearchPosition] );
        }

        this.Value = this.SearchOptions[ this.SearchPosition ]?.Term ?? "";
    }


    private async Task OnInputSearch_UI_Async( string? termText ) {
        if( this.Disabled ) {
            return;
        }

        if( termText is not null ) {
            IEnumerable<TermEntry> terms = await this.DbAccess.GetTermsByCriteria_Async(
                new ClientDbAccess.GetTermsByCriteriaParams( termText, null )
            );
            this.SearchOptions = terms.ToList();    // TODO
        } else {
            this.SearchOptions = new List<TermEntry>();
        }
    }


    private async Task SelectSearchResults_UI_Async( TermEntry term ) {
        if( this.Disabled ) {
            return;
        }
        if( !this.IsSeachFocused ) {
            return;
        }

        await this.SelectSearchResults_Async( term );
    }

    private async Task SelectSearchResults_Async( TermEntry term ) {
        this.Value = term.Term ?? "";

        await this.OnTermSelect_Async.Invoke( term );

       // await this.Js.InvokeVoidAsync(
       //     "window.TermInputComponent.SetTermSearchResult",
       //     new object[] { this.InputElement, term.ToString() ?? "" }
       //);
    }
}
