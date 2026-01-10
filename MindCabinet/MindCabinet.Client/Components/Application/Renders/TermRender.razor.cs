using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects.Term;


namespace MindCabinet.Client.Components.Application.Renders;


public partial class TermRender : ComponentBase {
    //[Inject]
    //public IJSRuntime Js { get; set; } = null!;

    [Inject]
    private ClientDataAccess_UserFavoriteTerms UserFavoriteTermsData { get; set; } = null!;


    [Parameter]
    public string? AddedClasses { get; set; } = null;


    [Parameter]
    public bool HasFavoriteButton { get; set; } = false;

    [Parameter]
    public RenderFragment? ChildContent { get; set; } = null;


    [Parameter, EditorRequired]
	public TermObject Term { get; set; } = null!;

	[Parameter]
	public Func<MouseEventArgs, Task>? OnClick_Async { get; set; } = null;

    

    public async Task<bool> CurrentTermIsFavorite_Async() {
        // TODO: Add caching
        IEnumerable<long> termIds = await this.UserFavoriteTermsData.GetTermIdsForCurrentUser_Async();
        return termIds.Contains( this.Term.Id );
    }


    private async Task ToggleFavoriteTerm_Async() {
        IEnumerable<long> termIds = await this.UserFavoriteTermsData.GetTermIdsForCurrentUser_Async();

        if( termIds.Contains(this.Term.Id) ) {
            await this.UserFavoriteTermsData.RemoveTermsForCurrentUser_Async(
                new ClientDataAccess_UserFavoriteTerms.RemoveTermsForCurrentUser_Params( [this.Term.Id] )
            );
        } else {
            await this.UserFavoriteTermsData.AddTermsForCurrentUser_Async(
                new ClientDataAccess_UserFavoriteTerms.AddTermsForCurrentUser_Params( [this.Term.Id] )
            );
        }
    }
}