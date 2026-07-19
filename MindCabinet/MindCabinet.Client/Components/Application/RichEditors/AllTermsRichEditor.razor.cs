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


    [Parameter]
    public string? AddedClasses { get; set; } = null;


    private List<TermObject> _Terms = new List<TermObject>();
    public IReadOnlyList<TermObject> Terms => this._Terms.AsReadOnly();


    [Parameter]
    public string? Label { get; set; } = null;


    public delegate Task OnTermsChange_Func(
        IEnumerable<TermObject> currentTerms,
        TermObject changedTerm,
        bool isAdded
    );

    [Parameter, EditorRequired]
    public OnTermsChange_Func OnTermsChange_Async { get; set; } = null!;


    public delegate Task OnTermReordered_Func(
        TermObject terms,
        int offset
    );

    [Parameter]
    public OnTermReordered_Func? OnTermReordered_Async { get; set; } = null!;



	protected async override Task OnInitializedAsync() {
		await base.OnInitializedAsync();

        TermObject.Raw[] terms = ( await this.TermsDataSrc
            .GetByCriteria_Async( new ClientDataAccess_Terms.IAPI.GetByCriteria_Params { } ) )
            .Terms
            .ToArray();

        this._Terms = (await ClientDataAccess_Terms.ConvertRawsToDataObjects_Async( this.TermsDataSrc, terms ))
            .ToList();
	}


    public async Task<bool> AddTerm_Async( TermObject term ) {
        if( this.Terms.Any(t => t.Equals(term)) ) {
            return false;
        }

        this._Terms.Add( term );

        await this.OnTermsChange_Async( this.Terms, term, true );

        return true;
    }
    

    public async Task<bool> RemoveTerm_Async( TermObject term ) {
        int idx = this._Terms.IndexOf( term );

        //if( !this.Terms.Any(t => t.Equals(term)) ) {
        if( idx == -1 ) {
            return false;
        }

        this._Terms.RemoveAt( idx );

        await this.OnTermsChange_Async( this.Terms, term, false );

		return true;
	}
}
