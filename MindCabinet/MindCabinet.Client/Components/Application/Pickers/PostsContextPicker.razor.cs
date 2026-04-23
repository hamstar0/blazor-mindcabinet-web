using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsContext;
using MindCabinet.Shared.Utility;


namespace MindCabinet.Client.Components.Application.Pickers;


public partial class PostsContextPicker : ComponentBase {
    //[Inject]
    //private IJSRuntime Js { get; set; } = null!;

    [Inject]
    private ClientDataAccess_Terms TermsData { get; set; } = null!;

    [Inject]
    private ClientDataAccess_PostsContext PostsContextsData { get; set; } = null!;

    [Inject]
    private ClientSessionData Session { get; set; } = null!;


    [Parameter, EditorRequired]
    public string UniqueName { get; set; } = null!;

    [Parameter]
    public string? AddedClasses { get; set; } = null;


    [Parameter, EditorRequired]
    public PostsContextObject[] Contexts { get; set; } = [];
    private PostsContextObject[] PreviousContexts = [];


    [Parameter, EditorRequired]
    public PostsContextObject CurrentContext { get; set; } = null!;


    private bool IsSeachFocused = false;

    private string Value = "";

    private bool IsCurrentInputSuppressed = false;

    private List<PostsContextObject> SearchOptions = new List<PostsContextObject>();

    private int SearchPosition = -1;


    [Parameter]
    public bool Disabled { get; set; } = false;

    [Parameter, EditorRequired]
    public Func<PostsContextObject, Task> OnContextSelect_Async { get; set; } = null!;



    protected async override Task OnParametersSetAsync() {
        await base.OnParametersSetAsync();

        bool contextsChanged = this.Contexts.Length != this.PreviousContexts.Length
            || this.Contexts
                .Where( (c, idx) => c.Id == this.PreviousContexts[idx].Id )
                .Count() != this.Contexts.Length;
        if( contextsChanged ) {
            this.PreviousContexts = this.Contexts;

            await this.LoadContextsIntoSearchOptions_Async();
        }
    }

    private async Task LoadContextsIntoSearchOptions_Async() {
        this.Value = this.CurrentContext?.Name ?? "";   // sorta blindly trusting this!

        await this.SearchAndStoreTerms_Async( this.Value );
    }


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

        this.Value = this.SearchOptions[ this.SearchPosition ]?.Name ?? "";
    }


    private async Task SearchAndStoreTerms_Async( string contextText ) {
        await Task.CompletedTask;

        if( this.Session.UserId is null ) {
            return;
        }

        if( contextText == "" ) {
            this.SearchOptions = this.Contexts.ToList();

            return;
        }

        this.SearchOptions = this.Contexts
            .Where( c => c.Name?
                .Contains( contextText, StringComparison.InvariantCultureIgnoreCase ) == true )
            .ToList();

        if( this.SearchOptions.Count > 0 ) {
            this.SearchPosition = this.SearchOptions
                .FindIndex( c => c.Id == this.CurrentContext?.Id ); // sorta blindly trusting this!
        } else {
            this.SearchPosition = -1;
        }
    }


    private async Task SelectSearchResults_Async( PostsContextObject context ) {
        this.Value = context.Name ?? "";

        await this.OnContextSelect_Async.Invoke( context );

        // await this.Js.InvokeVoidAsync(
        //     "window.TermInputComponent.SetTermSearchResult",
        //     new object[] { this.InputElement, term.ToString() ?? "" }
        //);
        
        this.StateHasChanged();
    }
}
