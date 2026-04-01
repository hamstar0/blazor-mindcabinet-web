using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserPostsContext;


namespace MindCabinet.Client.Components.Application.Pickers;


public partial class UserPostsContextPicker : ComponentBase {
    //[Inject]
    //private IJSRuntime Js { get; set; } = null!;

    [Inject]
    private ClientDataAccess_Terms TermsData { get; set; } = null!;

    [Inject]
    private ClientDataAccess_UserPostsContext UserPostsContextsData { get; set; } = null!;

    [Inject]
    private ClientSessionData Session { get; set; } = null!;


    [Parameter, EditorRequired]
    public string UniqueName { get; set; } = null!;

    [Parameter]
    public string? AddedClasses { get; set; } = null;


    private bool IsSeachFocused = false;

    private string Value = "";

    private bool IsCurrentInputSuppressed = false;

    private List<UserPostsContextObject> SearchOptions = new List<UserPostsContextObject>();

    private int SearchPosition = -1;


    [Parameter]
    public bool Disabled { get; set; } = false;

    [Parameter, EditorRequired]
    public Func<UserPostsContextObject, Task> OnContextSelect_Async { get; set; } = null!;


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
        if( this.Session.UserId is null ) {
            return;
        }

        ClientDataAccess_UserPostsContext.Get_Return contexts = await this.UserPostsContextsData.GetForCurrentUserByCriteria_Async(
            new ClientDataAccess_UserPostsContext.GetForCurrentUserByCriteria_Params {
                NameContains = contextText
            }
        );
        this.SearchOptions = (await ClientDataAccess_UserPostsContext.ToObjects_Async( this.TermsData, contexts.Contexts.ToArray() ))
            .ToList();
    }


    private async Task SelectSearchResults_Async( UserPostsContextObject context ) {
        this.Value = context.Name ?? "";

        await this.OnContextSelect_Async.Invoke( context );

        // await this.Js.InvokeVoidAsync(
        //     "window.TermInputComponent.SetTermSearchResult",
        //     new object[] { this.InputElement, term.ToString() ?? "" }
        //);
        
        this.StateHasChanged();
    }
}
