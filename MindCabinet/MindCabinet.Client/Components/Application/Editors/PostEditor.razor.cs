using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Data;
using MindCabinet.Shared.DataEntries;


namespace MindCabinet.Client.Components.Application.Editors;


public partial class PostEditor : ComponentBase {
    //[Inject]
    //public IJSRuntime Js { get; set; } = null!;

    [Inject]
    public ClientDataAccess Data { get; set; } = null!;

    //[Inject]
    //public LocalData LocalData { get; set; } = null!;


    [Parameter]
    public string? AddedClasses { get; set; } = null;


    private string PostText = "";

    private IList<TermEntry> Tags = new List<TermEntry>();

    [Parameter, EditorRequired]
    public Func<PostEntry, Task> OnSubmit_Async { get; set; } = null!;



    private async Task OnInputHandler_UI_Async( string text ) {
        this.PostText = text ?? "";

        this.StateHasChanged();
    }

    private async Task OnTagsChangeHandler_UI_Async( IList<TermEntry> tags, TermEntry changedTag, bool isAdded ) {
        this.Tags = tags;

        this.StateHasChanged();
    }

    private bool CanSubmit() {
        return this.PostText.Length >= 4
            && this.Tags.Count >= 1;
    }

    private async Task Submit_UI_Async() {
        PostEntry post = await this.Data.CreatePost_Async(
            new ClientDataAccess.CreatePostParams( this.PostText, this.Tags )
        );

        this.PostText = "";
        this.Tags.Clear();

        await this.OnSubmit_Async( post );
    }
}