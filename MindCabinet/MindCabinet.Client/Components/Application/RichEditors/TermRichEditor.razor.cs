using System.Drawing;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserTermFavorite;


namespace MindCabinet.Client.Components.Application.RichEditors;


public partial class TermRichEditor : ComponentBase {
    [Inject]
    private ClientDataAccess_Terms TermDataSrc { get; set; } = null!;

    [Inject]
    private ClientDataAccess_UserTermFavorites UserTermFavoritesDataSrc { get; set; } = null!;

    [Inject]
    private LocalClientSessionManager MySessionMngr { get; set; } = null!;


    [Parameter]
    public RenderFragment? PostFix { get; set; } = null;


    [Parameter]
    public string? AddedClasses { get; set; } = null;

    [Parameter]
    public string? AddedStyles { get; set; } = null;


    [Parameter, EditorRequired]
	public TermObject InitialTerm { get; set; } = null!;

	[Parameter]
	public Func<MouseEventArgs, Task>? OnClick_Async { get; set; } = null;

    public delegate Task<bool> OnDragFunc_Async( double x, double y, Rectangle rect );

	[Parameter]
	public OnDragFunc_Async? OnDrag_Async { get; set; } = null;



	protected override void OnParametersSet() {
		base.OnParametersSet();

        this.TermValue = this.InitialTerm.Term;
        this.AbbreviationValue = this.InitialTerm.Abbreviation ?? "";
        this.DescriptionValue = this.InitialTerm.Description ?? "";
        this.ContextTermValue = this.InitialTerm.Context?.Term ?? "";
        // this.AliasTermValue = this._Term.Alias?.Term;
	}

    public async Task<bool> CurrentTermIsFavorite_Async() {
        if( this.MySessionMngr.UserId is null ) {
            return false;
        }

        // TODO: Add caching
        IEnumerable<UserTermFavoriteObject.Raw> termRaws = await this.UserTermFavoritesDataSrc.GetFavTermsForCurrentUser_Async();
        return termRaws.Any( t => t.FavTermId == this.InitialTerm.Id );
    }


    private async Task ToggleFavoriteTerm_Async() {
        if( this.MySessionMngr.UserId is null ) {
            return;
        }

        IEnumerable<UserTermFavoriteObject.Raw> termRaws = await this.UserTermFavoritesDataSrc.GetFavTermsForCurrentUser_Async();

        if( termRaws.Any(t => t.FavTermId == this.InitialTerm.Id) ) {
            await this.UserTermFavoritesDataSrc.RemoveTermsForCurrentUser_Async(
                new ClientDataAccess_UserTermFavorites.IAPI.EditForCurrentUser_Params { TermIds = [this.InitialTerm.Id] }
            );
        } else {
            await this.UserTermFavoritesDataSrc.AddTermsForCurrentUser_Async(
                new ClientDataAccess_UserTermFavorites.IAPI.EditForCurrentUser_Params { TermIds = [this.InitialTerm.Id] }
            );
        }
    }
}