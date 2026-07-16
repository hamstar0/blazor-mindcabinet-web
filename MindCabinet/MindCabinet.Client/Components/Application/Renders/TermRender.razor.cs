using System.Drawing;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserTermFavorite;


namespace MindCabinet.Client.Components.Application.Renders;


public partial class TermRender : ComponentBase {
    [Inject]
    public IJSRuntime Js { get; set; } = null!;

    [Inject]
    private ClientDataAccess_UserTermFavorites UserTermFavoritesDataSrc { get; set; } = null!;

    [Inject]
    private LocalClientSessionManager MySessionMngr { get; set; } = null!;


    [Parameter]
    public string? AddedClasses { get; set; } = null;

    [Parameter]
    public string? AddedStyles { get; set; } = null;


    [Parameter]
    public bool VerboseTermDisplay { get; set; } = false;

    [Parameter]
    public bool HasFavoriteButton { get; set; } = false;

    [Parameter]
    public RenderFragment? ChildContent { get; set; } = null;


    [Parameter, EditorRequired]
	public TermObject Term { get; set; } = null!;

	[Parameter]
	public Func<MouseEventArgs, Task>? OnClick_Async { get; set; } = null;

    public delegate Task<bool> OnDragFunc_Async( double x, double y, Rectangle rect );

	[Parameter]
	public OnDragFunc_Async? OnDrag_Async { get; set; } = null;


    private ElementReference ComponentElement;

    

    private async Task<bool> HandleDrag_Async( MouseEventArgs e ) {
        if( e.ClientX == this.DragStartX && e.ClientY == this.DragStartY ) {
            return false;
        }

        Rectangle rect = await this.Js.InvokeAsync<Rectangle>(
            identifier: "getBoundingClientRect",
            this.ComponentElement
        );

        return await this.OnDrag_Async!( e.ClientX, e.ClientY, rect );
    }


    public async Task<bool> CurrentTermIsFavorite_Async() {
        if( this.MySessionMngr.UserId is null ) {
            return false;
        }

        // TODO: Add caching
        IEnumerable<UserTermFavoriteObject.Raw> termRaws = await this.UserTermFavoritesDataSrc.GetFavTermsForCurrentUser_Async();
        return termRaws.Any( t => t.FavTermId == this.Term.Id );
    }


    private async Task ToggleFavoriteTerm_Async() {
        if( this.MySessionMngr.UserId is null ) {
            return;
        }

        IEnumerable<UserTermFavoriteObject.Raw> termRaws = await this.UserTermFavoritesDataSrc.GetFavTermsForCurrentUser_Async();

        if( termRaws.Any(t => t.FavTermId == this.Term.Id) ) {
            await this.UserTermFavoritesDataSrc.RemoveTermsForCurrentUser_Async(
                new ClientDataAccess_UserTermFavorites.IAPI.RemoveTermsForCurrentUser_Params { TermIds = [this.Term.Id] }
            );
        } else {
            await this.UserTermFavoritesDataSrc.AddTermsForCurrentUser_Async(
                new ClientDataAccess_UserTermFavorites.IAPI.AddTermsForCurrentUser_Params { TermIds = [this.Term.Id] }
            );
        }
    }
}