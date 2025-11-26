using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MindCabinet.Shared.DataObjects.Term;


namespace MindCabinet.Client.Components.Application.Editors;


public partial class TagSetEditor : ComponentBase {
    public delegate Task OnTagsChange_Func( IReadOnlyList<TermObject> currentTags, TermObject changedTag, bool isAdded );


    private List<TermObject> _Tags = new List<TermObject>();

    public IReadOnlyList<TermObject> Tags => this._Tags.AsReadOnly();


    [Parameter]
    public string? AddedClasses { get; set; } = null;

    [Parameter]
    public string? Label { get; set; } = null;

    [Parameter]
    public bool AllowFavoritingTerms { get; set; } = true;


    [Parameter, EditorRequired]
    public OnTagsChange_Func OnTagsChange_Async { get; set; } = null!;



    public async Task AddTag_Async( TermObject tag ) {
        if( this.Tags.Any(t => t.Equals(tag)) ) {
            return;
        }

        this._Tags.Add( tag );

        await this.OnTagsChange_Async( this.Tags, tag, true );
    }
    

    public async Task<bool> RemoveTag_Async( TermObject tag ) {
        int idx = this._Tags.IndexOf( tag );

        //if( !this.Tags.Any(t => t.Equals(tag)) ) {
        if( idx == -1 ) {
            return false;
        }

        this._Tags.RemoveAt( idx );

        await this.OnTagsChange_Async( this.Tags, tag, false );

		return true;
	}
}