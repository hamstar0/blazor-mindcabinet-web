using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Components.Standard;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MindCabinet.Client.Components.SiteFunctions;


public partial class UserRegistrationForm : ComponentBase {
    public delegate Task OnUserCreateFunc_Async( SimpleUserObject.ClientObject user );



    //[Inject]
    //public IJSRuntime Js { get; set; } = null!;

    [Inject]
    private ClientDataAccess_SimpleUsers SimpleUsersData {get; set; } = null!;

    [Inject]
    private ClientSessionData SessionData { get; set; } = null!;


    private string MyModalId => "UserRegistrationForm_"+this.Id;    //Guid.NewGuid().ToString("N").Substring(0, 8);

    [Parameter, EditorRequired]
    public string Id { get; set; } = null!;

    [Parameter]
    public string? AddedClasses { get; set; } = null;
    
    [Parameter]
    public string? AddedStyles { get; set; } = null;

    private string UserName = "";

    private string Email = "";

    public string Password = "";

    [Parameter, EditorRequired]
    public OnUserCreateFunc_Async OnUserCreate_Async { get; set; } = null!;

    private string RegistrationStatus = "";

    private bool HasRegistered = false;


    private Modal? ModalDialogComponent = null;



    private SimpleUserObject.StatusCode GetSubmitStatusCode() {
        SimpleUserObject.StatusCode code = SimpleUserObject.GetUserNameStatus( this.UserName );
        code |= SimpleUserObject.GetEmailStatus( this.Email );
        code |= SimpleUserObject.GetPasswordStatus( this.Password );
        code |= this.SessionData.IsLoaded ? 0 : SimpleUserObject.StatusCode.NO_SESSION;
        return code;
    }


    public bool CanSubmit() {
        //this.GetSubmitStatusCode() == 0 ? false : true
        return this.GetSubmitStatusCode() == SimpleUserObject.StatusCode.OK
            && !this.HasRegistered;
    }
    
    private async Task<(bool success, string status)> Submit_Async( string userName, string email, string password ) {
        SimpleUserObject.StatusCode code = this.GetSubmitStatusCode();
        if( code != SimpleUserObject.StatusCode.OK ) {
            return (false, "Invalid input");
        }

        ClientDataAccess_SimpleUsers.Create_Return ret = await this.SimpleUsersData.Create_Async(
            new ClientDataAccess_SimpleUsers.Create_Params { 
                Name = userName,
                Email = email,
                Password = password,
                IsValidated = false
            }
        );

        if( ret.User is not null ) {
            await this.OnUserCreate_Async( ret.User );
        }

        return (ret.User is not null, ret.Status);
    }
}