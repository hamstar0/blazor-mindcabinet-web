using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Components.Application.Renders;
using MindCabinet.Client.Components.Standard;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Client.Services.DbAccess.Joined;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Client.Components.Application;


public partial class SimplePostsBrowser : ComponentBase {
    //[Inject]
    //public IJSRuntime Js { get; set; } = null!;

    [Inject]
    public ClientDataAccess_PrioritizedPosts PostsData { get; set; } = null!;

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

	private List<TermObject> AddedFilterTags = new List<TermObject>();


    private IEnumerable<SimplePostObject> CurrentPagePosts_Cache = [];
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


    public async Task<IEnumerable<SimplePostObject>> GetPostsOfCurrentPage_Async() { //todo: remove async/await?
        var search = new ClientDataAccess_SimplePosts.GetByCriteria_Params(
            bodyPattern: this.SearchTerm,
            allTags: new HashSet<TermObject>( this.AddedFilterTags ),
            sortAscendingByDate: this.SortAscendingByDate,
            pageNumber: this.CurrentPageNumber,
            postsPerPage: this.MaxPostsPerPage
        );
        IEnumerable<SimplePostObject> posts = await this.PostsData.GetByCriteria_Async( search );

//Console.WriteLine( "GetPostsOfCurrentPage_Async " + posts.Count() + ", " + search.ToString() );
        return posts;
    }

    public async Task<(int totalPosts, int totalPages)> GetTotalPostPagesCount_Async() {
        var search = new ClientDataAccess_SimplePosts.GetByCriteria_Params(
            bodyPattern: this.SearchTerm,
            allTags: new HashSet<TermObject>( this.AddedFilterTags ),
            sortAscendingByDate: this.SortAscendingByDate,
            pageNumber: 0,
            postsPerPage: -1
        );
        int totalPosts = await this.PostsData.GetCountByCriteria_Async( search );

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

    public async Task SetFilterTags_Async( IEnumerable<TermObject> changedTags ) {
        var currentTagStrings = new HashSet<string>( this.AddedFilterTags.Select(t=>t.ToString()) );
        var changedTagStrings = new HashSet<string>( changedTags.Select(t=>t.ToString()) );

// Console.WriteLine( "Console: SetFilterTags_Async"
//     +", tag:" + tag.ToString()
//     +", isAdded:" + isAdded
//     +", currentTags:" + string.Join(", ", currentTags.Select(t=>t.ToString()))
//     +", changedTags:" + string.Join(", ", changedTags.Select(t=>t.ToString()))
//     +", SetEquals:" + currentTags.SetEquals(changedTags) );
        if( currentTagStrings.SetEquals(changedTagStrings) ) {
//Console.WriteLine( " equal" );
            return;
        }

        this.CurrentPageNumber = 0;
        this.AddedFilterTags = changedTags.ToList();

        await this.RefreshPosts_Async();
    }
}