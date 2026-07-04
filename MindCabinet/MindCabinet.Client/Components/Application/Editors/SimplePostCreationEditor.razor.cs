using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Components.Standard;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;


namespace MindCabinet.Client.Components.Application.Editors;


public partial class SimplePostCreationEditor : ComponentBase {
    [Inject]
    private ClientDataAccess_SimplePosts SimplePostsDataSrc { get; set; } = null!;


    [Parameter]
    public string? AddedClasses { get; set; } = null;


    private string PostText = "";

    private List<TermObject> Tags = new List<TermObject>();

    [Parameter, EditorRequired]
    public Func<SimplePostObject.Raw, Task> OnSubmit_Async { get; set; } = null!;


	private TermSetEditor TagsEditor = null!;



    private void Reset() {
        this.PostText = "";
        this.TagsEditor.Reset();
        this.Tags.Clear();

        this.StateHasChanged();
    }


    private async Task OnInputHandler_UI_Async( string text ) {
        await Task.CompletedTask;
        this.PostText = text ?? "";

        this.StateHasChanged();
    }

    private async Task OnTagsChangeHandler_UI_Async( IEnumerable<TermObject> tags, TermObject changedTag, bool isAdded ) {
        await Task.CompletedTask;

        this.Tags = tags.ToList();

        this.StateHasChanged();
    }

    private bool CanSubmit() {
        return this.PostText.Length >= 4
            && this.Tags.Count >= 1;
    }

    private async Task Submit_UI_Async() {
        string text = this.PostText;
        TermId[] termIds = this.Tags.Select( t => t.Id ).ToArray();

        this.Reset();
        
        SimplePostObject.Raw post = await this.SimplePostsDataSrc.Create_Async(
            new ClientDataAccess_SimplePosts.IAPI.Create_Params {
                Body = text,
                TermIds = termIds
            }
        );

        await this.OnSubmit_Async( post );
    }
}