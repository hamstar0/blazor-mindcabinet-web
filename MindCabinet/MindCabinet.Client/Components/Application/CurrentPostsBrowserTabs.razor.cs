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


    private Tabs Tabs = null!;

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
        if( this.Tabs is null ) {
            this.TabsDataSuppliers.Insert( 0, ctx );

            return;
        }
        
        this.TabsDataSuppliers.Insert( this.Tabs.CurrentTabIndex, ctx );

        //

        PostsBrowser browser;
        (RenderFragment header, RenderFragment content) = this.GenerateTab_Blazor(
            ctx: ctx,
            postsBrowser: out browser
        );

        this.Tabs.InsertTab( header, content, this.Tabs.CurrentTabIndex );

        //

        this._PostsBrowsersByTabIndex.Insert( this.Tabs.CurrentTabIndex, browser );

        this.StateHasChanged();
    }
}
