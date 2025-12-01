using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;


namespace MindCabinet.Client.Components.Application.Editors;


public partial class CurrentContextEditor : ComponentBase {
    //[Inject]
    //public IJSRuntime Js { get; set; } = null!;

    // [Inject]
    // public ClientDbAccess DbAccess { get; set; } = null!;

    //[Inject]
    //public LocalData LocalData { get; set; } = null!;


    [Parameter]
    public string? AddedClasses { get; set; } = null;

    private List<TermObject> Tags = new List<TermObject>();

    [Parameter]
    public Func<List<TermObject>, Task>? OnSubmit_Async { get; set; } = null;


    public bool CanSubmit() {
        return this.Tags.Count > 0;
    }
    
    private async Task Submit_UI_Async() {
        this.Tags.Clear();

        add to database

        if( this.OnSubmit_Async is not null ) {
            await this.OnSubmit_Async.Invoke( this.Tags );
        }
    }
}