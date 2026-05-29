using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MindCabinet.Client.Components.Standard;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects.PostsContext;
using MindCabinet.Shared.DataObjects.Term;


namespace MindCabinet.Client.Components.Application.Pickers;



public partial class PostsContextPicker : ComponentBase {
    private string Value = "";


    [Inject]
    public ClientDataAccess_Terms TermsData { get; set; } = null!;

    [Inject]
    public ClientDataAccess_PostsContext PostsContextData { get; set; } = null!;


    [Parameter]
    public string? AddedClasses { get; set; } = null;


    private bool IsSeachFocused = false;

    private IEnumerable<PostsContextObject> SearchOptions = new List<PostsContextObject>();

    // private int SearchPosition = -1;


    [Parameter]
    public bool Disabled { get; set; } = false;

    [Parameter]
    public string? Description { get; set; }


    [Parameter, EditorRequired]
    public PostsContextObject[] InitialContexts { get; set; } = [];


    [Parameter, EditorRequired]
    public PostsContextObject InitialCurrentContext { get; set; } = null!;


    public delegate Task OnContextPickedFunc_Async( PostsContextObject context );

    [Parameter, EditorRequired]
    public OnContextPickedFunc_Async OnContextPicked_Async { get; set; } = null!;



    protected async override Task OnInitializedAsync() {
        await base.OnInitializedAsync();

        this.Value = this.InitialCurrentContext?.Name ?? "";   // sorta blindly trusting this!

        // await this.TrySearchContext_Async( this.Value );
        this.SearchOptions = this.InitialContexts.ToList();
    }


    private async Task TrySearchContext_Async( string searchText ) {
        if( searchText.Length == 0 ) {
            this.SearchOptions = new List<PostsContextObject>();
            // this.SearchPosition = 0;

            return;
        }

        IEnumerable<PostsContextObject.Raw> ctxsRaw = (await this.PostsContextData.GetForCurrentUserByCriteria_Async(
            new ClientDataAccess_PostsContext.IAPI.GetByCriteria_Params { NameContains = searchText }
        )).Contexts;

        this.SearchOptions = await ClientDataAccess_PostsContext
            .ConvertRawsToDataObjects_Async( this.TermsData, ctxsRaw.ToArray() );
    }

    private async Task SelectSearchResults_UI_Async( PostsContextObject context ) {
        if( this.Disabled ) {
            return;
        }
        //if( !this.IsSeachFocused ) {
        //    return;
        //}

        this.SearchOptions = new List<PostsContextObject>();
        // this.SearchPosition = 0;
        this.Value = context.Name;

        await this.OnContextPicked_Async( context );

        this.StateHasChanged();
    }
}
