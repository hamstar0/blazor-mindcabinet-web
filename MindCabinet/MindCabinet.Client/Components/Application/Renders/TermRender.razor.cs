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
        report if current term is a favorite via data access service 
    }


    private async Task ToggleFavoriteTerm_Async() {
        toggle current term as favorite via data access service
    }
}