using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Components.Application.Editors;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.PostsContext;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MindCabinet.Client.Components.Layout;


public partial class SidePanel {
    private PostsContextEditor PostsContextEditorComponent { get; set; } = null!;


    [Inject]
    private LocalClientSessionManager MySessionMngr { get; set; } = null!;

    [Inject]
    private ClientDataAccess_Terms TermsDataSrc { get; set; } = null!;

    [Inject]
    private ClientDataAccess_PostsContext PostsContextsDataSrc { get; set; } = null!;

    [Inject]
    private ClientDataAccess_UserAppData UserAppDataSrc { get; set; } = null!;



    [Parameter]
    public Func<Task>? OnStateChange_Async { get; set; } = null;

    private PostsContextObject[] Contexts_Cache = [];



	protected override async Task OnInitializedAsync() {
		await base.OnInitializedAsync();

        await this.MySessionMngr.RegisterPostsContextEvent_Async(
            name: "Sidebar",
            callback: async ctxMaybe => this.StateHasChanged()
        );
	}

	protected override async Task OnParametersSetAsync() {
		await base.OnParametersSetAsync();

        this.Contexts_Cache = await this.GetContexts_Async();
	}


    private async Task<PostsContextObject[]> GetContexts_Async() {
        if( this.MySessionMngr.UserId is null ) {
            return [];
        }
        
        PostsContextObject.Raw[] ctxs = (await this.PostsContextsDataSrc.GetForCurrentUserByCriteria_Async(
            new ClientDataAccess_PostsContext.IAPI.GetByCriteria_Params {
                NameContains = null,
                Ids = []
            }
        ) ).Contexts.ToArray();
        
        return await ClientDataAccess_PostsContext.ConvertRawsToDataObjects_Async(
            this.TermsDataSrc,
            ctxs
        );
    }
    
    private async Task SetContext_Async( PostsContextObject context ) {
        await this.MySessionMngr.SetCurrentContext_Await( this.UserAppDataSrc, context );
        
        this.PostsContextEditorComponent.SetDefaultContext( context );

        if( this.OnStateChange_Async is not null ) {
            await this.OnStateChange_Async.Invoke();
        }
    }
}
