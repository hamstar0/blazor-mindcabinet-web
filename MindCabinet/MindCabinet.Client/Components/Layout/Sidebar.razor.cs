using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.PostsContext;
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
    private ClientDataAccess_Terms TermsData { get; set; } = null!;

    [Inject]
    private ClientDataAccess_PostsContext PostsContextsData { get; set; } = null!;


    [Parameter]
    public Func<Task>? OnStateChange_Async { get; set; } = null;

    private PostsContextObject[] Contexts_Cache = [];



	protected override async Task OnInitializedAsync() {
		await base.OnInitializedAsync();

        await this.SessionData.RegisterPostsContextEvent_Async( "Sidebar", async ctxMaybe => this.StateHasChanged() );
	}

	protected override async Task OnParametersSetAsync() {
		await base.OnParametersSetAsync();

        this.Contexts_Cache = await this.GetContexts_Async();
	}


    private async Task<PostsContextObject[]> GetContexts_Async() {
        if( this.SessionData.UserId is null ) {
            return [];
        }
        
        PostsContextObject.Raw[] ctxs = (await this.PostsContextsData.GetForCurrentUserByCriteria_Async(
            new ClientDataAccess_PostsContext.GetForCurrentUserByCriteria_Params {
                NameContains = null,
                Ids = []
            }
        ) ).Contexts.ToArray();
        
        return await ClientDataAccess_PostsContext.ConvertRawsToDataObjects_Async(
            this.TermsData,
            ctxs
        );
    }
}
