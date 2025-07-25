using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Components.Application.Renders;
using MindCabinet.Client.Components.Standard;
using MindCabinet.Client.Services;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Client.Components.Application;


public partial class PostsBrowser : ComponentBase {
    //[Inject]
    //public IJSRuntime Js { get; set; } = null!;

    [Inject]
    public ClientDbAccess DbAccess { get; set; } = null!;

    //[Inject]
    //public LocalData LocalData { get; set; } = null!;


    [Parameter]
    public string? AddedClasses { get; set; } = null;


    [Parameter]
    public int MaxPostsPerPage { get; set; } = 5;

    [Parameter]
    public int MaxPagesToDisplay { get; set; } = 10;


    private int CurrentPageNumber = 0;

    private bool SortAscendingByDate = false;

    private string SearchTerm = "";

	private IList<TermObject> FilterTags = new List<TermObject>();


    private IEnumerable<PostObject> CurrentPagePosts_Cache = [];
	private int TotalPages_Cache;
	private int TotalPosts_Cache;



    protected override async Task OnInitializedAsync() {
        await this.RefreshPosts_Async();
    }

    public async Task RefreshPosts_Async() {
        var task1 = this.GetPostsOfCurrentPage_Async();
        var task2 = this.GetTotalPostPagesCount_Async();
        await Task.WhenAll( task1, task2 );

        this.CurrentPagePosts_Cache = await task1;
        (this.TotalPosts_Cache, this.TotalPages_Cache) = await task2;

        if( this.CurrentPageNumber >= this.TotalPages_Cache ) {
            this.CurrentPageNumber = this.TotalPages_Cache - 1;
        }

        this.StateHasChanged();
    }


    public async Task<IEnumerable<PostObject>> GetPostsOfCurrentPage_Async() { //todo: remove async/await?
        var search = new ClientDbAccess.GetPostsByCriteriaParams(
            bodyPattern: this.SearchTerm,
            tags: new HashSet<TermObject>( this.FilterTags ),
            sortAscendingByDate: this.SortAscendingByDate,
            pageNumber: this.CurrentPageNumber,
            postsPerPage: this.MaxPostsPerPage
        );
        IEnumerable<PostObject> posts = await this.DbAccess.GetPostsByCriteria_Async( search );

//Console.WriteLine( "GetPostsOfCurrentPage_Async " + posts.Count() + ", " + search.ToString() );
        return posts;
    }

    public async Task<(int totalPosts, int totalPages)> GetTotalPostPagesCount_Async() {
        int totalPosts = await this.DbAccess.GetPostCountByCriteria_Async( new ClientDbAccess.GetPostsByCriteriaParams(
            bodyPattern: this.SearchTerm,
            tags: new HashSet<TermObject>( this.FilterTags ),
            sortAscendingByDate: this.SortAscendingByDate,
            pageNumber: 0,
            postsPerPage: -1
        ) );

        return (totalPosts, ( int)Math.Ceiling( (float)totalPosts / (float)this.MaxPostsPerPage ));
    }


    public async Task ChangePage_Async( int page ) {
        int maxPages = this.TotalPages_Cache > 0 ? this.TotalPages_Cache - 1 : 0;
        page = Math.Clamp( page, 0, maxPages );

        if( page == this.CurrentPageNumber ) {
            return;
        }

        this.CurrentPageNumber = page;

        await this.RefreshPosts_Async();
    }

    public async Task SetCreateDateSort_Async( bool isAscending ) {
        if( isAscending == this.SortAscendingByDate ) {
            return;
        }

        this.CurrentPageNumber = 0;
        this.SortAscendingByDate = isAscending;

        await this.RefreshPosts_Async();
    }

    public async Task SetBodySearch_Async( string term ) {
        if( term == this.SearchTerm ) {
            return;
        }

        this.CurrentPageNumber = 0;
        this.SearchTerm = term;

        await this.RefreshPosts_Async();
    }

    public async Task SetFilterTags_Async( IList<TermObject> alreadyChangedTags ) {
        var currentTags = new HashSet<string>( this.FilterTags.Select(t=>t.ToString()) );
        var changedTags = new HashSet<string>( alreadyChangedTags.Select(t=>t.ToString()) );

//Console.WriteLine( "SetFilterTags_Async " + string.Join(", ", tags.Select(t=>t.ToString())) );
        if( currentTags.SetEquals(changedTags) ) {
//Console.WriteLine( " equal" );
            return;
        }

        this.CurrentPageNumber = 0;
        this.FilterTags = alreadyChangedTags;

        await this.RefreshPosts_Async();
    }
}