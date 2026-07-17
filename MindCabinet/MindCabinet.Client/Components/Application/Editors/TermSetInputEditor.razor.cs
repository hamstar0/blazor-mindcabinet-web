using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MindCabinet.Shared.DataObjects.Term;


namespace MindCabinet.Client.Components.Application.Editors;


public partial class TermSetInputEditor : ComponentBase {
    [Parameter]
    public List<TermObject> InitialTerms { get; set; } = new List<TermObject>();
    private List<TermObject> InitialTermsSnapshot = new List<TermObject>();

    private List<TermObject> _Terms = new List<TermObject>();
    public IReadOnlyList<TermObject> Terms => this._Terms.AsReadOnly();


    [Parameter]
    public bool AdjustableOrderAndVerticalStackMode { get; set; } = false;


    [Parameter]
    public string? AddedClasses { get; set; } = null;

    [Parameter]
    public string? AddedStyle { get; set; } = null;

    [Parameter]
    public string? AddedPerItemClasses { get; set; } = null;

    [Parameter]
    public bool VerboseTermDisplay { get; set; } = false;

    [Parameter]
    public string? Label { get; set; } = null;

    [Parameter]
    public bool AllowFavoritingTerms { get; set; } = true;


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



	protected override void OnParametersSet() {
		base.OnParametersSet();

        if( this.InitialTermsSnapshot != this.InitialTerms ) {
            this.InitialTermsSnapshot = this.InitialTerms;

            this._Terms = new List<TermObject>( this.InitialTerms );
        }
    }


    public void Reset() {
        this._Terms = this.InitialTerms.ToList();

        this.StateHasChanged();
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


    public async Task<bool> ShiftTermOrder_Async( TermObject term, int offset ) {
        int idx = this._Terms.IndexOf( term );
        if( idx == -1 ) {
            throw new ArgumentException( "Invalid Term "+term.ToString() );
        }

        if( idx + offset < 0 ) {
            return false;
        } else if( idx + offset >= this._Terms.Count ) {
            return false;
        }

        TermObject tmp = this._Terms[ idx + offset ];
        this._Terms[ idx + offset ] = term;
        this._Terms[ idx ] = tmp;

        if( this.OnTermReordered_Async is not null ) {
            await this.OnTermReordered_Async( term, offset );
        }

        return true;
    }
}