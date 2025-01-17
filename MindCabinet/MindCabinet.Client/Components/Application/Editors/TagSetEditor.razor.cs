using Microsoft.AspNetCore.Components;
using MindCabinet.Shared.DataEntries;


namespace MindCabinet.Client.Components.Application.Editors;


public partial class TagSetEditor : ComponentBase {
    public delegate Task OnTagsChange_Func( IList<TermEntry> currentTags, TermEntry changedTag, bool isAdded );


    private IList<TermEntry> Tags = new List<TermEntry>();


    [Parameter]
    public string? AddedClasses { get; set; } = null;

    [Parameter]
    public string? Label { get; set; } = null;


    [Parameter, EditorRequired]
    public OnTagsChange_Func OnTagsChange_Async { get; set; } = null!;



    public async Task AddTag_Async( TermEntry tag ) {
        if( this.Tags.Any(t => t.Equals(tag)) ) {
            return;
        }

        this.Tags.Add( tag );

        await this.OnTagsChange_Async( this.Tags, tag, true );
    }
    

    public async Task<bool> RemoveTag_Async( TermEntry tag ) {
        int idx = this.Tags.IndexOf( tag );

        //if( !this.Tags.Any(t => t.Equals(tag)) ) {
        if( idx == -1 ) {
            return false;
        }

        this.Tags.RemoveAt( idx );

        await this.OnTagsChange_Async( this.Tags, tag, false );

		return true;
	}
}