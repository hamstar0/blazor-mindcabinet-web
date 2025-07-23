using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;


namespace MindCabinet.Client.Components.Application.Editors;


public partial class PostEditor : ComponentBase {
    //[Inject]
    //public IJSRuntime Js { get; set; } = null!;

    [Inject]
    public ClientDbAccess DbAccess { get; set; } = null!;

    //[Inject]
    //public LocalData LocalData { get; set; } = null!;


    [Parameter]
    public string? AddedClasses { get; set; } = null;


    private string PostText = "";

    private IList<TermObject> Tags = new List<TermObject>();

    [Parameter, EditorRequired]
    public Func<PostObject, Task> OnSubmit_Async { get; set; } = null!;



    private async Task OnInputHandler_UI_Async( string text ) {
        this.PostText = text ?? "";

        this.StateHasChanged();
    }

    private async Task OnTagsChangeHandler_UI_Async( IList<TermObject> tags, TermObject changedTag, bool isAdded ) {
        this.Tags = tags;

        this.StateHasChanged();
    }

    private bool CanSubmit() {
        return this.PostText.Length >= 4
            && this.Tags.Count >= 1;
    }

    private async Task Submit_UI_Async() {
        PostObject post = await this.DbAccess.CreatePost_Async(
            new ClientDbAccess.CreatePostParams( this.PostText, this.Tags )
        );

        this.PostText = "";
        this.Tags.Clear();

        await this.OnSubmit_Async( post );
    }
}