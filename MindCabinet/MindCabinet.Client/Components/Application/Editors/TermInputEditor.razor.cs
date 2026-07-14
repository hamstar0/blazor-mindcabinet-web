using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MindCabinet.Client.Components.Standard;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects.Term;


namespace MindCabinet.Client.Components.Application.Editors;



public partial class TermInputEditor : ComponentBase {
    private string Value = "";


    [Inject]
    public ClientDataAccess_Terms TermsDataSrc { get; set; } = null!;


    [Parameter]
    public string? AddedClasses { get; set; } = null;

    [Parameter]
    public bool VerboseTermDisplay { get; set; } = false;


    private bool IsSeachFocused = false;

    private IEnumerable<TermObject> SearchOptions = new List<TermObject>();

    // private int SearchPosition = -1;


    [Parameter]
    public bool Disabled { get; set; } = false;

    //[Parameter]
    //public string Label { get; set; } = "";
    
    [Parameter]
    public string? Description { get; set; }


    public delegate Task<(string termText, bool isSubmit)> OnTermInputFunc_Async( string termText );
    public delegate Task OnTermConfirmFunc_Async( TermObject term, bool isAdded );

    [Parameter]
    public OnTermInputFunc_Async? OnAttemptingTermInput_Async { get; set; } = null;

    [Parameter, EditorRequired]
    public OnTermConfirmFunc_Async OnTermConfirm_Async { get; set; } = null!;
    

    private Modal TermContext_ModalDialogComponent = null!;

    private Modal Error_ModalDialogComponent = null!;



    private async Task<bool> BeginSubmitNewTerm_Async( string termText ) {
        await Task.CompletedTask;

        if( !TermObject.ValidateTerm(termText) ) {
            this.Error_ModalDialogComponent.Open();
            
            return false;
        }

        this.TermContext_ModalDialogComponent.Open();

        return true;
    }

    private async Task<bool> SubmitNewTerm_Async( string termText, TermObject? contextTerm ) {
        ClientDataAccess_Terms.IAPI.Create_Return newTermRet = await this.TermsDataSrc.Create_Async(
            new ClientDataAccess_Terms.IAPI.Create_Params { TermPattern = termText, ContextId = contextTerm?.Id, AliasId = null }
        );

        TermObject newTerm = await ClientDataAccess_Terms.ConvertRawToDataObject_Async( this.TermsDataSrc, newTermRet.TermRaw );

        await this.OnTermConfirm_Async( newTerm, newTermRet.IsAdded );

        return newTermRet.IsAdded;
    }


    private async Task TrySearchTerm_Async( string termText ) {
        if( termText.Length == 0 ) {
            this.SearchOptions = new List<TermObject>();
            // this.SearchPosition = 0;

            return;
        }

        IEnumerable<TermObject.Raw> termsRaw = (await this.TermsDataSrc.GetByCriteria_Async(
            new ClientDataAccess_Terms.IAPI.GetByCriteria_Params { TermPattern = termText!, ContextTermId = null, ContextTermPattern = null }
        )).Terms;

        IEnumerable<Task<TermObject>> termTasks = termsRaw.Select(
            t => t.ToDataObject_Async(
                async t => (await this.TermsDataSrc.GetByIds_Async( new TermId[] { t } ))
                    .Terms
                    .First()
            )
        );

        this.SearchOptions = await Task.WhenAll( termTasks );
    }

    private async Task SelectSearchResults_UI_Async( TermObject term ) {
        if( this.Disabled ) {
            return;
        }
        //if( !this.IsSeachFocused ) {
        //    return;
        //}

        this.SearchOptions = new List<TermObject>();
        // this.SearchPosition = 0;
        this.Value = term.Term;

        await this.OnTermConfirm_Async( term, false );

        this.StateHasChanged();
    }
}
