using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;


namespace MindCabinet.Client.Components.Application.Editors;


public partial class SimplePostEditor : ComponentBase {
    //[Inject]
    //private IJSRuntime Js { get; set; } = null!;

    [Inject]
    private ClientDataAccess_SimplePosts SimplePostsData { get; set; } = null!;

    //[Inject]
    //private LocalData LocalData { get; set; } = null!;


    [Parameter]
    public string? AddedClasses { get; set; } = null;


    private string PostText = "";

    private List<TermObject> Tags = new List<TermObject>();

    [Parameter, EditorRequired]
    public Func<SimplePostObject.Raw, Task> OnSubmit_Async { get; set; } = null!;



    private async Task OnInputHandler_UI_Async( string text ) {
        await Task.CompletedTask;
        this.PostText = text ?? "";

        this.StateHasChanged();
    }

    private async Task OnTagsChangeHandler_UI_Async( IEnumerable<TermObject> tags, TermObject changedTag, bool isAdded ) {
        this.Tags = tags.ToList();

        this.StateHasChanged();
    }

    private bool CanSubmit() {
        return this.PostText.Length >= 4
            && this.Tags.Count >= 1;
    }

    private async Task Submit_UI_Async() {
        SimplePostObject.Raw post = await this.SimplePostsData.Create_Async(
            new ClientDataAccess_SimplePosts.Create_Params { Body = this.PostText, TermIds = this.Tags.Select( t => t.Id ).ToArray() }
        );

        this.PostText = "";
        this.Tags.Clear();

        await this.OnSubmit_Async( post );
    }
}