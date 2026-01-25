using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.UserContext;
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
    private ClientDataAccess_UserContext UserContextsData { get; set; } = null!;

    private IEnumerable<UserContextObject>? Contexts = null;
    private UserContextObject? CurrentContext = null;


    
    protected override async Task OnInitializedAsync() {
        await base.OnInitializedAsync();

        if( this.SessionData.UserId is null ) {
            return;
        }

        this.Contexts = await this.UserContextsData.GetForCurrentUserByCriteria_Async(
            new ClientDataAccess_UserContext.GetForCurrentUserByCriteria_Params()
        );

        UserContextObject? currentContext = this.SessionData.GetCurrentContext();

        if( currentContext is not null ) {
            this.CurrentContext = this.Contexts
                .FirstOrDefault( c => c.Id == currentContext.Id );
        }
    }

    private async Task OnContextSelect_Async( UserContextObject context ) {
        this.SessionData.SetCurrentContext( context );
        
        this.CurrentContext = context;
    }
}
