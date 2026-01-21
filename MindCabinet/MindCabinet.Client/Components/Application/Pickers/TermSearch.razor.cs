using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects.Term;


namespace MindCabinet.Client.Components.Application.Pickers;


public partial class TermSearch : ComponentBase {
    //[Inject]
    //private IJSRuntime Js { get; set; } = null!;

    [Inject]
    private ClientDataAccess_Terms TermsData { get; set; } = null!;

    [Inject]
    private ClientDataAccess_UserFavoriteTerms UserFavoriteTermsData { get; set; } = null!;

    [Inject]
    private ClientDataAccess_UserTermsHistory UserTermsHistoryData { get; set; } = null!;

    [Inject]
    private ClientSessionData Session { get; set; } = null!;


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


    private IEnumerable<TermObject> FavoriteTerms_Cache = new List<TermObject>();
    private IEnumerable<TermObject> RecentTerms_Cache = new List<TermObject>();



    protected override async Task OnInitializedAsync() {
        IEnumerable<long> favTermIds
            = await this.UserFavoriteTermsData.GetTermIdsForCurrentUser_Async();
        IEnumerable<ClientDataAccess_UserTermsHistory.GetTermIdsForCurrentUser_Return> histTermIds
            = await this.UserTermsHistoryData.GetTermIdsForCurrentUser_Async();

        this.FavoriteTerms_Cache = await this.TermsData.GetByIds_Async(
            favTermIds
        );
        this.RecentTerms_Cache = await this.TermsData.GetByIds_Async(
            histTermIds
                .OrderByDescending( x => x.Created )
                .Select( x => x.TermId )
        );
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

        this.Value = this.SearchOptions[ this.SearchPosition ]?.Term ?? "";
    }


    private async Task SearchAndStoreTerms_Async( string termText ) {
        IEnumerable<TermObject> terms = await this.TermsData.GetByCriteria_Async(
            new ClientDataAccess_Terms.GetByCriteria_Params( termText, null )
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
