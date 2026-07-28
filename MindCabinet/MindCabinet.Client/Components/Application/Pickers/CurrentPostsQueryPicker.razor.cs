using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MindCabinet.Client.Components.Standard;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects.PostsQuery;
using MindCabinet.Shared.DataObjects.Term;


namespace MindCabinet.Client.Components.Application.Pickers;



public partial class CurrentPostsQueryPicker : ComponentBase {
    public string Value { get; private set; } = "";


    [Inject]
    public ClientDataAccess_Terms TermsDataSrc { get; set; } = null!;

    [Inject]
    public ClientDataAccess_PostsQuery PostsQueryDataSrc { get; set; } = null!;


    [Parameter]
    public string? AddedClasses { get; set; } = null;


    private bool IsSearchFocused = false;

    private IEnumerable<PostsQueryObject> SearchOptions = new List<PostsQueryObject>();

    // private int SearchPosition = -1;


    [Parameter]
    public bool Disabled { get; set; } = false;

    [Parameter]
    public string? Description { get; set; }


    [Parameter, EditorRequired]
    public PostsQueryObject[] InitialSearchOptionsCache { get; set; } = [];       // hackish


    [Parameter, EditorRequired]
    public PostsQueryObject InitialSelectedSearchOption { get; set; } = null!;    // hackish


    public delegate Task OnQueryPickedFunc_Async( PostsQueryObject context );

    [Parameter, EditorRequired]
    public OnQueryPickedFunc_Async OnQueryPicked_Async { get; set; } = null!;



    protected async override Task OnInitializedAsync() {
        await base.OnInitializedAsync();

        this.Value = this.InitialSelectedSearchOption?.Name ?? "";   // sorta blindly trusting this!

        // await this.TrySearchQuery_Async( this.Value );
        this.SearchOptions = this.InitialSearchOptionsCache.ToList();

        CurrentPostsQueryPicker.Main = this;
    }


    private async Task<IEnumerable<PostsQueryObject>> GetQueriesFromSearch_Async( string search ) {
        if( search.Length == 0 ) {
            // this.SearchPosition = 0;
            return new List<PostsQueryObject>();
        }

        IEnumerable<PostsQueryObject.Raw> queriesRaw = (await this.PostsQueryDataSrc.GetForCurrentUserByCriteria_Async(
            new ClientDataAccess_PostsQuery.IAPI.GetByCriteria_Params { NameContains = this.Value }
        )).Queries;

        return await ClientDataAccess_PostsQuery
            .ConvertRawsToDataObjects_Async( this.TermsDataSrc, queriesRaw.ToArray() );
    }
}
