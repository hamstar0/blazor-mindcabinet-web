using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MindCabinet.Client.Components.Application.Renders;
using MindCabinet.Client.Data;
using MindCabinet.Shared.DataEntries;
using static MindCabinet.Client.Data.ClientDataAccess;


namespace MindCabinet.Client.Components.Application.Editors;



public partial class TermEditor : ComponentBase {
    public delegate Task<TermEntry?> OnTermConfirmFunc_Async( TermEntry term, bool isAdded );



    private string Value = "";


    //[Inject]
    //public IJSRuntime Js { get; set; } = null!;

    [Inject]
    public ClientDataAccess Data { get; set; } = null!;


    [Parameter]
    public string? AddedClasses { get; set; } = null;


    private bool IsSeachFocused = false;

    private IEnumerable<TermEntry> SearchOptions = new List<TermEntry>();


    [Parameter]
    public bool Disabled { get; set; } = false;

    //[Parameter]
    //public string Label { get; set; } = "";
    
    [Parameter]
    public string? Description { get; set; }

    [Parameter]
    public Func<string, Task<(string, bool)>>? OnTermInput_Async { get; set; } = null;

    [Parameter, EditorRequired]
    public OnTermConfirmFunc_Async OnTermConfirm_Async { get; set; } = null!;



    private async Task OnInputKey_UI_Async( KeyboardEventArgs arg ) {
        if( this.Disabled ) {
            return;
        }
        if( !this.IsSeachFocused ) {
            return;
        }

 
        if( arg.Code == "Enter" || arg.Code == "NumpadEnter" ) {
            await this.TrySubmitNewTerm_Async( this.Value );

            return;
        }

        //Console.WriteLine("key input: "+(int)arg.Key[0]+", value: "+this.Value );
        //todo arrow key search results navigation
    }


    private async Task OnInputTermOrSearch_UI_Async( string termText ) {
        if( this.Disabled ) {
            return;
        }
        
        if( string.IsNullOrEmpty(termText) ) {
            this.SearchOptions = new List<TermEntry>();
            this.Value = "";

            return;
        }

        bool isSubmit = false;

        if( this.OnTermInput_Async is not null ) {
            (termText, isSubmit) = await this.OnTermInput_Async( termText! );
        }

        this.Value = termText!;

        if( isSubmit ) {
            await this.TrySubmitNewTerm_Async( termText! );
        } else {
            await this.TrySearchTerm_Async( termText! );
        }
    }


    private async Task<bool> TrySubmitNewTerm_Async( string termText ) {
        if( termText.Length < 2 ) {
            return false;
        }

        this.SearchOptions = new List<TermEntry>();

        return await this.SubmitNewTerm_Async( termText! );
    }
    private async Task<bool> SubmitNewTerm_Async( string termText ) {
        ClientDataAccess.CreateTermReturn newTermRet = await this.Data.CreateTerm_Async(
            new ClientDataAccess.CreateTermParams( termText, null, null )
        );

        TermEntry? newerTerm = await this.OnTermConfirm_Async( newTermRet.Term, newTermRet.IsAdded );

        if( newerTerm is null ) {
            this.Value = "";

            this.StateHasChanged();
        } else if( !newerTerm.Equals(newTermRet.Term) ) {
            this.Value = newerTerm.Term;

            this.StateHasChanged();
        }

        return newTermRet.IsAdded;
    }


    private async Task TrySearchTerm_Async( string termText ) {
        if( termText.Length == 0 ) {
            this.SearchOptions = new List<TermEntry>();

            return;
        }

        this.SearchOptions = await this.Data.GetTermsByCriteria_Async(
            new ClientDataAccess.GetTermsByCriteriaParams( termText!, null )
        );
    }

    private async Task SelectSearchResults_UI_Async( TermEntry term ) {
//Console.WriteLine( "SelectSearchResults_UI_Async "+term.ToString()+", "+this.Disabled );
        if( this.Disabled ) {
            return;
        }
        //if( !this.IsSeachFocused ) {
        //    return;
        //}

        this.SearchOptions = new List<TermEntry>();
        this.Value = term.Term;

        await this.OnTermConfirm_Async( term, false );

        this.StateHasChanged();
    }
}
