using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Components.Application.Renders;
using MindCabinet.Client.Components.Standard;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DataPresenters;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Client.Services.DbAccess.Joined;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsContext;
using System.ComponentModel;


namespace MindCabinet.Client.Components.Application;


public partial class CurrentPostsBrowserTabs : ComponentBase {
    //[Inject]
    //public IJSRuntime Js { get; set; } = null!;

    [Inject]
    public PostsSupplier PostsData { get; set; } = null!;

    [Inject]
    public LocalClientSessionManager MySessionMngr { get; set; } = null!;

    [Inject]
    public ClientDataAccess_Terms TermsDataSrc { get; set; } = null!;


    [Parameter, EditorRequired]
    public string Id { get; set; } = null!;
    
    [Parameter]
    public string? AddedClasses { get; set; } = null;



    protected override async Task OnInitializedAsync() {
        await this.RefreshPosts_Async();
    }

    public async Task RefreshPosts_Async() {
        this.CurrentPagePosts_Cache = await this.GetPostsOfCurrentPage_Async();

        (this.TotalPosts_Cache, this.TotalPages_Cache) = await this.GetTotalPostPagesCount_Async();

        int currPage = this.PostsData.GetCurrentPage();
        if( currPage >= this.TotalPages_Cache ) {
            this.PostsData.SetCurrentPage( Math.Max(0, this.TotalPages_Cache - 1) );
        }

        this.StateHasChanged();
    }


    public async Task<IEnumerable<SimplePostObject>> GetPostsOfCurrentPage_Async() {
        PostsContextObject? context = this.MySessionMngr.GetCurrentContext();
        if( context is null ) {
            return [];
        }

        IEnumerable<SimplePostObject> posts = await this.PostsData.GetCurrentContextPosts_Async(
            termsDataSrc: this.TermsDataSrc,
            searchTerm: this.SearchTerm,
            addedFilterTagIds: this.AddedFilterTags.Select( t => t.Id ).ToArray()
        );

//Console.WriteLine( "GetPostsOfCurrentPage_Async " + posts.Count() + ", " + search.ToString() );
        return posts;
    }

    public async Task<(int totalPosts, int totalPages)> GetTotalPostPagesCount_Async() {
        int totalPosts = await this.PostsData.GetCurrentContextPostCount_Async(
            searchTerm: this.SearchTerm,
            addedFilterTagIds: this.AddedFilterTags.Select( t => t.Id ).ToArray()
        );

        return (totalPosts, (int)Math.Ceiling( (float)totalPosts / (float)this.PostsData.GetMaxPostsPerPage() ) );
    }
}