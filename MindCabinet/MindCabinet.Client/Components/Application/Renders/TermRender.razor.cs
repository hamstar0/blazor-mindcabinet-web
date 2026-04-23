using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserTermFavorite;


namespace MindCabinet.Client.Components.Application.Renders;


public partial class TermRender : ComponentBase {
    //[Inject]
    //public IJSRuntime Js { get; set; } = null!;

    [Inject]
    private ClientDataAccess_UserTermFavorites UserTermFavoritesData { get; set; } = null!;

    [Inject]
    private ClientSessionManager Session { get; set; } = null!;


    [Parameter]
    public string? AddedClasses { get; set; } = null;


    [Parameter]
    public bool HasFavoriteButton { get; set; } = false;

    [Parameter]
    public RenderFragment? ChildContent { get; set; } = null;


    [Parameter, EditorRequired]
	public TermObject Term { get; set; } = null!;

    [Parameter]
	public TermObject? TermContextContext { get; set; } = null;

	[Parameter]
	public Func<MouseEventArgs, Task>? OnClick_Async { get; set; } = null;

    

    public async Task<bool> CurrentTermIsFavorite_Async() {
        if( this.Session.UserId is null ) {
            return false;
        }

        // TODO: Add caching
        IEnumerable<UserTermFavoriteObject.Raw> termRaws = await this.UserTermFavoritesData.GetFavTermsForCurrentUser_Async();
        return termRaws.Any( t => t.FavTermId == this.Term.Id );
    }


    private async Task ToggleFavoriteTerm_Async() {
        if( this.Session.UserId is null ) {
            return;
        }

        IEnumerable<UserTermFavoriteObject.Raw> termRaws = await this.UserTermFavoritesData.GetFavTermsForCurrentUser_Async();

        if( termRaws.Any(t => t.FavTermId == this.Term.Id) ) {
            await this.UserTermFavoritesData.RemoveTermsForCurrentUser_Async(
                new ClientDataAccess_UserTermFavorites.RemoveTermsForCurrentUser_Params { TermIds = [this.Term.Id] }
            );
        } else {
            await this.UserTermFavoritesData.AddTermsForCurrentUser_Async(
                new ClientDataAccess_UserTermFavorites.AddTermsForCurrentUser_Params { TermIds = [this.Term.Id] }
            );
        }
    }
}