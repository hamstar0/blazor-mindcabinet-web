using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Components.Application.Renders;
using MindCabinet.Client.Components.Standard.Tabs2;
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


    private List<PostsContextObject> TabsDataSuppliers = new();




    public async Task RefreshBrowsers_Async() {
        foreach( PostsBrowser browser in this._PostsBrowsersByTabIndex ) {
            await browser.RefreshPosts_Async();
        }

        this.StateHasChanged();
    }


    public void NewTabAtCurrentIndex( PostsContextObject ctx ) {
        this.TabsDataSuppliers.Insert( this.Tabs?.CurrentTabIndex ?? 0, ctx );

        this.StateHasChanged();
    }
}
