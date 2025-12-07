using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserContext;


namespace MindCabinet.Client.Components.Application.Pickers;


public partial class UserContextPicker : ComponentBase {
    //[Inject]
    //private IJSRuntime Js { get; set; } = null!;

    [Inject]
    private ClientDataAccess_UserContext UserContextsData { get; set; } = null!;

    [Inject]
    private ClientSessionData Session { get; set; } = null!;


    [Parameter]
    public string? AddedClasses { get; set; } = null;


    private bool IsSeachFocused = false;

    private string Value = "";

    private bool IsCurrentInputSuppressed = false;

    private List<UserContextObject> SearchOptions = new List<UserContextObject>();

    private int SearchPosition = -1;


    [Parameter]
    public bool Disabled { get; set; } = false;

    [Parameter, EditorRequired]
    public Func<UserContextObject, Task> OnContextSelect_Async { get; set; } = null!;



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
        IEnumerable<UserContextObject> contexts = await this.UserContextsData.GetForCurrentUserByCriteria_Async(
            new ClientDataAccess_UserContext.GetForCurrentUserByCriteria_Params( contextText )
        );
        this.SearchOptions = contexts.ToList();    // TODO
    }


    private async Task SelectSearchResults_Async( UserContextObject context ) {
        this.Value = context.Name ?? "";

        await this.OnContextSelect_Async.Invoke( context );

       // await this.Js.InvokeVoidAsync(
       //     "window.TermInputComponent.SetTermSearchResult",
       //     new object[] { this.InputElement, term.ToString() ?? "" }
       //);
    }
}
