using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Components.Application;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DataPresenters;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects.Term;
using System.Text;

namespace MindCabinet.Client.Components.Application.RichEditors;


public partial class AllTermsRichEditor : ComponentBase {
    [Inject]
    private ClientDataAccess_Terms TermsDataSrc { get; set; } = null!;


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


    private bool DisplayListNeedsRefresh = true;
    
    [Parameter]
    public bool SortAscending { get; set; } = false;

    private string TermFilterValue = "";

    private TermId? ContextIdFilter = null;



	protected async override Task OnParametersSetAsync() {
		await base.OnParametersSetAsync();

        if( this.DisplayListNeedsRefresh ) {
            this.DisplayListNeedsRefresh = false;

            (this._Terms, int totalTerms) = await this.GetTerms_Async();
            this.TotalPages = (int)Math.Ceiling( (double)(totalTerms / this.PageSize) );
        }
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


    public async Task<bool> AddTerm_Async( TermObject term ) {
        if( this.Terms.Any(t => t.Equals(term)) ) {
            return false;
        }

        this._Terms.Add( term );

        if( this.OnTermsChange_Async is not null ) {
            await this.OnTermsChange_Async( this.Terms, term, true );
        }

        return true;
    }
    

    public async Task<bool> RemoveTerm_Async( TermObject term ) {
        int idx = this._Terms.IndexOf( term );

        //if( !this.Terms.Any(t => t.Equals(term)) ) {
        if( idx == -1 ) {
            return false;
        }

        this._Terms.RemoveAt( idx );

        if( this.OnTermsChange_Async is not null ) {
            await this.OnTermsChange_Async( this.Terms, term, false );
        }

		return true;
	}
}
