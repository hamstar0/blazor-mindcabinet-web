using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MindCabinet.Client.Components.SiteFunctions;


public partial class UserLoginForm : ComponentBase {
    public delegate Task OnUserLoginFunc_Async( SimpleUserObject.ClientData user );



    //[Inject]
    //private IJSRuntime Js { get; set; } = null!;

    [Inject]
    private ClientDataAccess_SimpleUsers UsersData { get; set; } = null!;


    [Parameter]
    public string? AddedClasses { get; set; } = null;

    [Parameter, EditorRequired]
    public string ModalId { get; set; } = null!;

    private string UserName = "";

    public string Password = "";

    [Parameter, EditorRequired]
    public OnUserLoginFunc_Async OnUserLogin_Async { get; set; } = null!;

    public string? LoginStatus = null;



    public bool CanSubmit() {
        return this.UserName.Length > 0 && this.Password.Length > 0;
    }

    private async Task<bool> Submit_UI_Async() {
        ClientDataAccess_SimpleUsers.Login_Return reply = await this.UsersData.Login_Async(
            new ClientDataAccess_SimpleUsers.Login_Params(
                name: this.UserName,
                password: this.Password
            )
        );

        if( reply.User is not null ) {
            await this.OnUserLogin_Async( reply.User );
            this.LoginStatus = null;
        } else {
            this.LoginStatus = reply.Status;
        }

        return reply.User is not null;
    }
}