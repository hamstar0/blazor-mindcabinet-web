using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MindCabinet.Shared.DataObjects.Term;


namespace MindCabinet.Client.Components.Application.Editors;


public partial class TermSetEditor : ComponentBase {
    public delegate Task OnTermsChange_Func( IEnumerable<TermObject> currentTerms, TermObject changedTerm, bool isAdded );



    
    [Parameter]
    public List<TermObject> InitialTerms { get; set; } = new List<TermObject>();

    private List<TermObject> _Terms = new List<TermObject>();

    public IReadOnlyList<TermObject> Terms => this._Terms.AsReadOnly();

    [Parameter]
    public string? AddedClasses { get; set; } = null;

    [Parameter]
    public string? Label { get; set; } = null;

    [Parameter]
    public bool AllowFavoritingTerms { get; set; } = true;


    [Parameter, EditorRequired]
    public OnTermsChange_Func OnTermsChange_Async { get; set; } = null!;



	protected override void OnParametersSet() {
		base.OnParametersSet();

        this._Terms = new List<TermObject>( this.InitialTerms );
    }

    public async Task AddTerm_Async( TermObject term ) {
        if( this.Terms.Any(t => t.Equals(term)) ) {
            return;
        }

        this._Terms.Add( term );
        await this.OnTermsChange_Async( this.Terms, term, true );
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