using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Components.Application.Editors;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.PostsQuery;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MindCabinet.Client.Components.Layout;


public partial class SidePanel {
    private PostsQueryEditor PostsQueryEditorComponent { get; set; } = null!;


    [Inject]
    private LocalClientSessionManager MySessionMngr { get; set; } = null!;

    [Inject]
    private ClientDataAccess_Terms TermsDataSrc { get; set; } = null!;

    [Inject]
    private ClientDataAccess_PostsQuery PostsQueryDataSrc { get; set; } = null!;

    [Inject]
    private ClientDataAccess_UserAppData UserAppDataSrc { get; set; } = null!;



    [Parameter]
    public Func<Task>? OnStateChange_Async { get; set; } = null;



	protected override async Task OnInitializedAsync() {
		await base.OnInitializedAsync();

        await this.MySessionMngr.RegisterPostsQueryEvent_Async(
            name: "Sidebar",
            callback: async queryMaybe => this.StateHasChanged()
        );
    }


    private async Task<PostsQueryObject[]> GetContexts_Async() {
        if( this.MySessionMngr.UserId is null ) {
            return [];
        }
        
        PostsQueryObject.Raw[] queries = (await this.PostsQueryDataSrc.GetForCurrentUserByCriteria_Async(
            new ClientDataAccess_PostsQuery.IAPI.GetByCriteria_Params {
                NameContains = null,
                Ids = []
            }
        ) ).Queries.ToArray();
        
        return await ClientDataAccess_PostsQuery.ConvertRawsToDataObjects_Async(
            this.TermsDataSrc,
            queries
        );
    }
    
    private async Task SetContext_Async( PostsQueryObject context ) {
        await this.MySessionMngr.SetCurrentContext_Await( this.UserAppDataSrc, context );
        
        this.PostsQueryEditorComponent.SetDefaultQuery( context );

        if( this.OnStateChange_Async is not null ) {
            await this.OnStateChange_Async.Invoke();
        }
    }
}
