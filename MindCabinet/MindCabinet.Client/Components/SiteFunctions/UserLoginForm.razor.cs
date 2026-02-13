using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Components.Standard;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MindCabinet.Client.Components.SiteFunctions;


public partial class UserLoginForm : ComponentBase {
    public delegate Task OnUserLoginFunc_Async( SimpleUserObject.ClientData user );



    //[Inject]
    //private IJSRuntime Js { get; set; } = null!;

    [Inject]
    private ClientDataAccess_SimpleUsers UsersData { get; set; } = null!;

    [Inject]
    private ClientSessionData SessionData { get; set; } = null!;


    private Modal ModalElement = null!;

    private string MyModalId => "UserLoginForm_"+this.Id;    //Guid.NewGuid().ToString("N").Substring(0, 8);

    [Parameter, EditorRequired]
    public string Id { get; set; } = null!;

    [Parameter]
    public string? AddedClasses { get; set; } = null;
    
    [Parameter]
    public string? AddedStyles { get; set; } = null;


    private string UserName = "";

    public string Password = "";


    [Parameter, EditorRequired]
    public OnUserLoginFunc_Async OnUserLogin_Async { get; set; } = null!;

    public string? LoginStatus = null;



    public bool CanSubmit() {
        return this.UserName.Length > 0 && this.Password.Length > 0;
    }


    private async Task<(bool success, string status)> Submit_Async( string userName, string password ) {
        ClientDataAccess_SimpleUsers.Login_Return reply = await this.UsersData.Login_Async(
            new ClientDataAccess_SimpleUsers.Login_Params(
                name: userName,
                password: password
            )
        );

        string status;

        if( reply.User is not null ) {
            await this.OnUserLogin_Async( reply.User );
            status = $"Welcome back, {reply.User.Name}!";
        } else {
            status = reply.Status;
        }

        return (reply.User is not null, status);
    }
}