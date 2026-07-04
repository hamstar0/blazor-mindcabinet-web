using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Components.Application.Renders;
using MindCabinet.Client.Components.Standard;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DataPresenters;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Client.Services.DbAccess.Joined;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsContext;
using System.ComponentModel;


namespace MindCabinet.Client.Components.Application;


public partial class CurrentPostsBrowserTabs : ComponentBase {
    //[Inject]
    //public IJSRuntime Js { get; set; } = null!;

    [Inject]
    public LocalClientSessionManager MySessionMngr { get; set; } = null!;


    [Parameter, EditorRequired]
    public string Id { get; set; } = null!;
    
    [Parameter]
    public string? AddedClasses { get; set; } = null;


    private PostsContextObject[] Contexts = [];



    protected override async Task OnInitializedAsync() {
        PostsContextObject? ctx = this.MySessionMngr.GetCurrentContext();

        if( ctx is not null ) {
            this.Contexts = [ ctx ];

            this.StateHasChanged();
        }
    }
}
