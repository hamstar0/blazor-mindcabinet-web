using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MindCabinet.Client.Components.Application.Renders;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserTermFavorite;
using MindCabinet.Shared.DataObjects.UserTermHistory;

namespace MindCabinet.Client.Components.Application.Pickers;


public partial class TermSearch : ComponentBase {
    //[Inject]
    //private IJSRuntime Js { get; set; } = null!;

    [Inject]
    private ClientDataAccess_Terms TermsData { get; set; } = null!;

    [Inject]
    private ClientDataAccess_UserTermFavorites UserTermFavoritesData { get; set; } = null!;

    [Inject]
    private ClientDataAccess_UserTermsHistory UserTermsHistoryData { get; set; } = null!;

    [Inject]
    private ClientSessionData Session { get; set; } = null!;


    private MultiTermRender SearchResultsElement = null!;


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


    private IEnumerable<UserTermFavoriteObject.ClientObject> FavoriteTerms_Cache = [];
    private IEnumerable<UserTermHistoryObject.ClientObject> RecentTerms_Cache = [];



	protected async override Task OnParametersSetAsync() {
        await base.OnParametersSetAsync();

        if( this.Session.UserId is not null ) {
            await this.InitializeTermOptions_Async();
        }
    }

    private async Task InitializeTermOptions_Async() {
        IEnumerable<UserTermFavoriteObject.Raw> favTerms
            = await this.UserTermFavoritesData.GetFavTermsForCurrentUser_Async();
        IEnumerable<UserTermHistoryObject.Raw> histTerms
            = await this.UserTermsHistoryData.GetHistTermsForCurrentUser_Async();

        this.FavoriteTerms_Cache = await ClientDataAccess_UserTermFavorites.ConvertRawsToClientObjects_Async(
            this.TermsData,
            favTerms.ToArray()
        );
        this.RecentTerms_Cache = await ClientDataAccess_UserTermsHistory.ConvertRawsToClientObjects_Async(
            this.TermsData,
            histTerms.ToArray()
        );
    }


    private async Task SearchTerms_Async( string termText ) {
        IEnumerable<TermObject.Raw> rawTerms = (await this.TermsData.GetByCriteria_Async(
            new ClientDataAccess_Terms.GetByCriteria_Params { TermPattern = termText, ContextTermId = null, ContextTermPattern = null }
        )).Terms;

        this.SearchOptions = (await ClientDataAccess_Terms.ConvertRawsToDataObjects_Async( this.TermsData, rawTerms.ToArray() ))
            .ToList();
    }


    private async Task SelectSearchResults_Async( TermObject term ) {
        this.Value = term.Term ?? "";

        this.SearchOptions = new List<TermObject>();

        await this.OnTermSelect_Async.Invoke( term );
    }
}
