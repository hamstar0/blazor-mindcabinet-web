using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services;
using MindCabinet.Shared.DataEntries;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MindCabinet.Client.Components.Site;


public partial class UserLoginForm : ComponentBase {
    public delegate Task OnUserLoginFunc_Async( SimpleUserEntry.ClientData user );



    //[Inject]
    //public IJSRuntime Js { get; set; } = null!;

    [Inject]
    public HttpClient Http { get; set; } = null!;

    [Inject]
    public ClientDbAccess DbAccess { get; set; } = null!;

    [Inject]
    public ClientSessionData SessionData { get; set; } = null!;


    [Parameter]
    public string? AddedClasses { get; set; } = null;

    [Parameter, EditorRequired]
    public string ModalId { get; set; } = null!;

    private string UserName = "";

    public string Password = "";

    [Parameter, EditorRequired]
    public OnUserLoginFunc_Async OnUserLogin_Async { get; set; } = null!;

    public string? LoginStatus = null;



    protected async override Task OnParametersSetAsync() {
        await base.OnParametersSetAsync();

        await this.SessionData.Load_Async( this.Http );
    }
    

    private async Task OnInputUserName_UI_Async( string val ) {
        this.UserName = val;
    }

    private async Task OnInputPassword_UI_Async( string val ) {
        this.Password = val;
    }


    public bool CanSubmit() {
        return this.UserName.Length > 0 && this.Password.Length > 0;
    }

    private async Task<bool> Submit_UI_Async() {
        ClientDbAccess.SimpleUserLoginReply reply = await this.DbAccess.LoginSimpleUser_Async(
            new ClientDbAccess.LoginSimpleUserParams(
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