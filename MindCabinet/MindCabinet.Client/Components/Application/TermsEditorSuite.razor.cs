using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Components.Application;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DataPresenters;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserTermFavorite;
using System.Text;

namespace MindCabinet.Client.Components.Application;


public partial class TermsEditorSuite : ComponentBase {
    [Inject]
    private ClientDataAccess_Terms TermsDataSrc { get; set; } = null!;

    [Inject]
    private ClientDataAccess_UserTermFavorites FavDataSrc { get; set; } = null!;


    [Parameter]
    public string? AddedClasses { get; set; } = null;


    private List<UserTermFavoriteObject.ClientObject> FavTerms_Cache = null!;



	protected async override Task OnInitializedAsync() {
		await base.OnInitializedAsync();
        
        UserTermFavoriteObject.Raw[] favRaws = (await this.FavDataSrc.GetFavTermsForCurrentUser_Async())
            .ToArray();
        this.FavTerms_Cache = (await ClientDataAccess_UserTermFavorites.ConvertRawsToClientObjects_Async( this.TermsDataSrc, favRaws ))
            .ToList();
	}
}
