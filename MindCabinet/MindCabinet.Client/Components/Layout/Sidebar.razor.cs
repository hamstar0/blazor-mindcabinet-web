using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.UserPostsContext;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MindCabinet.Client.Components.Layout;


public partial class Sidebar {
    //[Inject]
    //public IJSRuntime Js { get; set; } = null!;

    //[Inject]
    //public HttpClient Http { get; set; } = null!;

    //[Inject]
    //public ClientDbAccess DbAccess { get; set; } = null!;
    
    [Inject]
    private ClientSessionData SessionData { get; set; } = null!;

    [Inject]
    private ClientDataAccess_UserPostsContext UserPostsContextsData { get; set; } = null!;


    [Parameter]
    public Func<Task>? OnStateChange_Async { get; set; } = null;



	protected override async Task OnInitializedAsync() {
		await base.OnInitializedAsync();

        await this.SessionData.RegisterUserPostsContextEvent_Async( "Sidebar", async ctxMaybe => this.StateHasChanged() );
	}
}
