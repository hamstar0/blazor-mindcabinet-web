using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Components.Application;
using MindCabinet.Client.Components.Application.Pickers;
using MindCabinet.Client.Components.Layout;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DataPresenters;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects.PostsContext;
using MindCabinet.Shared.DataObjects.Term;
using System.Text;

namespace MindCabinet.Client.Components.Application.RichEditors;


public partial class AllTermsRichEditor : ComponentBase {
    [Inject]
    private LocalClientSessionManager MySessMngr { get; set; } = null!;

    [Inject]
    private TabNavigation Navigation { get; set; } = null!;

    [Inject]
    private ClientDataAccess_Terms TermsDataSrc { get; set; } = null!;

    [Inject]
    private ClientDataAccess_PostsContext PostsContextDataSrc { get; set; } = null!;


    [Parameter, EditorRequired]
    public string Id { get; set; } = null!;

    [Parameter]
    public string? AddedClasses { get; set; } = null;


    private List<TermObject> _Terms = [];
    public IReadOnlyList<TermObject> Terms => this._Terms.AsReadOnly();


    [Parameter]
    public string? Label { get; set; } = null;


    public delegate Task OnTermsChange_Func(
        IEnumerable<TermObject> currentTerms,
        TermObject changedTerm,
        bool isAdded
    );

    [Parameter]
    public OnTermsChange_Func? OnTermsChange_Async { get; set; } = null;

    
    [Parameter]
    public int PageSize { get; set; } = 20;

    public int CurrentPage { get; private set; } = 0;

    public int TotalPages;


    [Parameter]
    public bool SortAscending { get; set; } = false;

    private string TermFilterValue = "";

    private TermId? ContextIdFilter = null;


    private bool IsListLoaded = false;



	protected async override Task OnParametersSetAsync() {
		await base.OnParametersSetAsync();

        if( !this.IsListLoaded ) {
            this.IsListLoaded = true;
            
            await this.RefreshList_Async();
        }
	}


    private async Task RefreshList_Async() {
        (this._Terms, int totalTerms) = await this.GetTerms_Async();
        this.TotalPages = (int)Math.Ceiling( (double)(totalTerms / this.PageSize) );
    }


	private async Task<(List<TermObject> terms, int totalTerms)> GetTerms_Async() {
        var criteria = new ClientDataAccess_Terms.IAPI.GetByCriteria_Params {
            Page = this.CurrentPage,
            PageSize = this.PageSize,
            SortAscendingByTerm = this.SortAscending,
            ContextTermId = this.ContextIdFilter,
            TermPattern = this.TermFilterValue != "" ? this.TermFilterValue : null
        };
        IEnumerable<TermObject.Raw> rawTerms = ( await this.TermsDataSrc
            .GetByCriteria_Async( criteria ) )
            .Terms;

        List<TermObject> terms = (await ClientDataAccess_Terms.ConvertRawsToDataObjects_Async( this.TermsDataSrc, rawTerms ))
            .ToList();
        
        int totalTerms = await this.TermsDataSrc.GetCountByCriteria_Async( criteria );

        return (terms, totalTerms);
	}


    public async Task<bool> OnAddTerm_Async( TermObject term ) {
        if( this.Terms.Any(t => t.Equals(term)) ) {
            return false;
        }

        this._Terms.Add( term );

        if( this.OnTermsChange_Async is not null ) {
            await this.OnTermsChange_Async( this.Terms, term, true );
        }

        return true;
    }
    

    /* public async Task<bool> RemoveTerm_Async( TermObject term ) {
        int idx = this._Terms.IndexOf( term );

        //if( !this.Terms.Any(t => t.Equals(term)) ) {
        if( idx == -1 ) {
            return false;
        }

        if( !(await this.TermsDataSrc.RemoveForCurrentUser_Async(term.Id)) ) {
            return false;
        }

        this._Terms.RemoveAt( idx );

        if( this.OnTermsChange_Async is not null ) {
            await this.OnTermsChange_Async( this.Terms, term, false );
        }

		return true;
	} */


    private async Task ViewPostsOfTerm_Async( TermObject term ) {
        ClientDataAccess_PostsContext.IAPI.CreateOrUpdate_Return ret = await this.PostsContextDataSrc.CreateForCurrentUser_Async(
            new PostsContextObject.Prototype {
                Name = $"'{term.ToFullString()}' posts",
                Owner = this.MySessMngr.UserId,
                Entries = [
                    new PostsContextTermEntryObject.Prototype {
                        TermId = term.Id
                    }
                ]
            }
        );

        await CurrentPostsContextPicker.Main.PickPostContext_Async( ret.Id );

        this.Navigation.Navigate( $"{MainPanel.Main.PostsTabId}.{CurrentPostsBrowserTabs.Main.Id}" );
    }
}
