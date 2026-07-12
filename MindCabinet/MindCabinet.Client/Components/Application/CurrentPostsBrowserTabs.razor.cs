using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Components.Application.Renders;
using MindCabinet.Client.Components.Standard.Tabs;
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
    [Inject]
    public LocalClientSessionManager MySessionMngr { get; set; } = null!;


    private Tabs? Tabs = null;


    private List<PostsBrowser> _PostsBrowsersByTabIndex = new();



    [Parameter, EditorRequired]
    public string Id { get; set; } = null!;
    
    [Parameter]
    public string? AddedClasses { get; set; } = null;


    [Parameter, EditorRequired]
    public EachCurrentPostsSupplierSupplier TabsData { get; set; } = null!;

    private PostsSupplier[] Suppliers_Cache = null!;




	protected override async Task OnParametersSetAsync() {
        this.Suppliers_Cache = (await this.TabsData.GetPostsSuppliers_Async())
            .ToArray();

		await base.OnParametersSetAsync();
	}

    public async Task RefreshBrowsers_Async() {
        foreach( PostsBrowser browser in this._PostsBrowsersByTabIndex ) {
            await browser.RefreshPosts_Async();
        } verify

        this.StateHasChanged();
    }
}
