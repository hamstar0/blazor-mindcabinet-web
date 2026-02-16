using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Components.Standard;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MindCabinet.Client.Components.SiteFunctions;


public partial class UserRegistrationForm : ComponentBase {
    public delegate Task OnUserCreateFunc_Async( SimpleUserObject.ClientData user );



    //[Inject]
    //public IJSRuntime Js { get; set; } = null!;

    [Inject]
    private ClientDataAccess_SimpleUsers SimpleUsersData {get; set; } = null!;

    [Inject]
    private ClientSessionData SessionData { get; set; } = null!;


    private Modal ModalElement = null!;

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



    public enum StatusCode {
        OK = 0,
        NAME_EMPTY = 1,
        NAME_SHORT = 2,
        NAME_LONG = 4,
        NAME_WS_PADDED = 8,
        NAME_WS_CONSEC = 16,
        NAME_WS_MISC = 32,
        NAME_UNICODE = 64,
        EMAIL_EMPTY = 128,
        EMAIL_MALFORMED = 256,
        PW_EMPTY = 512,
        PW_SHORT = 1024,
        PW_LONG = 2048,
        PW_NO_NUM = 4096,
        PW_NO_UPPER = 8192,
        NO_SESSION = 16384
    }
    
    public static readonly IReadOnlyDictionary<StatusCode, string> StatusCodeMessages = new Dictionary<StatusCode, string> {
        { StatusCode.NAME_EMPTY, "User name cannot be empty." },
        { StatusCode.NAME_SHORT, "User name too short." },
        { StatusCode.NAME_LONG, "User name too long." },
        { StatusCode.NAME_WS_PADDED, "User name has whitespace padding." },
        { StatusCode.NAME_WS_CONSEC, "User name has consecutive whitespaces." },
        { StatusCode.NAME_WS_MISC, "User has invalid whitespace characters." },
        { StatusCode.NAME_UNICODE, "User name has unicode characters." },
        { StatusCode.EMAIL_EMPTY, "Email is empty." },
        { StatusCode.EMAIL_MALFORMED, "Email is malformed." },
        { StatusCode.PW_EMPTY, "Password is empty." },
        { StatusCode.PW_SHORT, "Password is too short." },
        { StatusCode.PW_LONG, "Password is too long." },
        { StatusCode.PW_NO_NUM, "Password is missing numbers." },
        { StatusCode.PW_NO_UPPER, "Password missing uppercase letters." },
        { StatusCode.NO_SESSION, "Session not loaded." },
    }.AsReadOnly();


    private StatusCode GetSubmitUserNameStatusCode() {
        if( string.IsNullOrEmpty(this.UserName) ) {
            return StatusCode.NAME_EMPTY;
        }

        StatusCode code = 0;

        if( this.UserName.Length < 3 ) {
            code |= StatusCode.NAME_SHORT;
        } else if( this.UserName.Length >= 32 ) {
            code |= StatusCode.NAME_LONG;
        }

        if( this.UserName[0] == ' ' || this.UserName[this.UserName.Length-1] == ' ' ) {
            code |= StatusCode.NAME_WS_PADDED;
        }

        int consecSpaces = 0;
        for( int i=0; i<this.UserName.Length; i++ ) {
            char c = this.UserName[i];
            if( c == ' ' ) {
                consecSpaces++;
            } else {
                consecSpaces = 0;
            }
            if( consecSpaces >= 2 ) {
                code |= StatusCode.NAME_WS_CONSEC;
            }
            if( c != ' ' && !Char.IsLetterOrDigit(c) ) {
                code |= StatusCode.NAME_WS_MISC;
            }
            if( c > 127 ) {
                code |= StatusCode.NAME_UNICODE;
            }
        }

        return code;
    }


    private StatusCode GetSubmitEmailStatusCode() {
        if( string.IsNullOrEmpty(this.Email) ) {
            return StatusCode.EMAIL_EMPTY;
        }

        return new EmailAddressAttribute().IsValid( this.Email )
            ? StatusCode.OK
            : StatusCode.EMAIL_MALFORMED;
    }

    private StatusCode GetSubmitPasswordStatusCode() {
        if( string.IsNullOrEmpty(this.Password) ) {
            return StatusCode.PW_EMPTY;
        }

        StatusCode code = 0;

        if( this.Password.Length < 5 ) {
            code |= StatusCode.PW_SHORT;
        } else if( this.Password.Length >= 1000 ) {
            code |= StatusCode.PW_LONG;
        }

        bool hasNumber = false;
        bool hasUpper = false;

        for( int i=0; i<this.Password.Length; i++ ) {
            char c = this.Password[i];
            hasNumber |= Char.IsDigit( c );
            hasUpper |= Char.IsUpper( c );
        }

        if( !hasNumber ) {
            code |= StatusCode.PW_NO_NUM;
        }
        if( !hasUpper ) {
            code |= StatusCode.PW_NO_UPPER;
        }

        return code;
    }

    private StatusCode GetSubmitStatusCode() {
        StatusCode code = this.GetSubmitUserNameStatusCode();
        code |= this.GetSubmitEmailStatusCode();
        code |= this.GetSubmitPasswordStatusCode();
        code |= this.SessionData.IsLoaded ? 0 : StatusCode.NO_SESSION;
        return code;
    }

    public List<string> GetSubmitStatuses( StatusCode code ) {
        var statuses = new List<string>();

        foreach( (StatusCode msgCode, string message) in UserRegistrationForm.StatusCodeMessages ) {
            if( (code & msgCode) != 0 ) {
                statuses.Add( message );
            }
        }

        return statuses;
    }


    public bool CanSubmit() {
        //this.GetSubmitStatusCode() == 0 ? false : true
        return this.GetSubmitStatusCode() == StatusCode.OK
            && !this.HasRegistered;
    }
    
    private async Task<(bool success, string status)> Submit_Async( string userName, string email, string password ) {
        StatusCode code = this.GetSubmitStatusCode();
        if( code != StatusCode.OK ) {
            return (false, "Invalid input");
        }

        ClientDataAccess_SimpleUsers.Create_Return ret = await this.SimpleUsersData.Create_Async(
            new ClientDataAccess_SimpleUsers.Create_Params(
                name: userName,
                email: email,
                password: password,
                isValidated: false
            )
        );

        if( ret.User is not null ) {
            await this.OnUserCreate_Async( ret.User );
        }

        return (ret.User is not null, ret.Status);
    }
}