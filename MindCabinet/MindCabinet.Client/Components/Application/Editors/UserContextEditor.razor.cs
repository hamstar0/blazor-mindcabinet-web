using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserContext;


namespace MindCabinet.Client.Components.Application.Editors;


public partial class UserContextEditor : ComponentBase {
    //[Inject]
    //private IJSRuntime Js { get; set; } = null!;

    //[Inject]
    //private ClientDbAccess DbAccess { get; set; } = null!;

    [Inject]
    private ClientDataAccess_Terms TermsData { get; set; } = null!;

    [Inject]
    private ClientDataAccess_UserContext UserContextsData { get; set; } = null!;

    [Inject]
    private ClientSessionData SessionsData { get; set; } = null!;


    [Parameter]
    public string? AddedClasses { get; set; } = null;

    [Parameter]
    public UserContextObject? InitialContext { get; set; } = null;

	private UserContextObject.Prototype CurrentContextPrototype = new UserContextObject.Prototype();

    [Parameter]
    public Func<UserContextObject.Prototype, Task>? OnCreate_Async { get; set; } = null;



	protected override void OnParametersSet() {
		base.OnParametersSet();
        
        if( this.InitialContext is not null ) {
            this.CurrentContextPrototype = new UserContextObject.Prototype {
                Name = this.InitialContext?.Name,
                Description = this.InitialContext?.Description,
                Entries = this.InitialContext?.Entries
                    .Select( e => e.ToRaw(this.InitialContext.Id) ).ToArray()
                    ?? []
            };
        }
	}


	private async Task AddNewTag_Async( TermObject newTag ) {
        this.CurrentContextPrototype.Entries.Append( new UserContextTermEntryObject.Raw {
            TermId = newTag.Id,
            Priority = 0d,
            IsRequired = false
        } );
	}

	private async Task RemoveTag_Async( TermObject newTag ) {
        List<UserContextTermEntryObject.Raw> entries = this.CurrentContextPrototype.Entries.ToList();

        for( int i = 0; i < entries.Count; i++ ) {
            if( entries[i].TermId == newTag.Id ) {
                entries.RemoveAt( i );
                break;
            }
        }

        this.CurrentContextPrototype.Entries = entries.ToArray();
	}

    
	private async Task<bool> HasUnsavedChanges() {
        UserContextObject? currentCtx = this.SessionsData.GetCurrentContext();
        
        if( currentCtx is null ) {
            return this.CurrentContextPrototype.Name is not null
                || this.CurrentContextPrototype.Description is not null
                || this.CurrentContextPrototype.Entries.Any();
        }

        ClientDataAccess_UserContext.Get_Return contexts = await this.UserContextsData.GetForCurrentUserByCriteria_Async(
            new ClientDataAccess_UserContext.GetForCurrentUserByCriteria_Params {
                Ids = [ currentCtx.Id ]
            }
        );

        UserContextObject.Raw currentContextRaw = contexts
            .Contexts
            .First( ctx => ctx.Id == currentCtx.Id );
        UserContextObject currentContext = await ClientDataAccess_UserContext.ToObject_Async( this.TermsData, currentContextRaw );

		return currentContext is not null
            ? !this.CurrentContextPrototype.Matches( currentContext! )
            : false;
	}


	private void ResetCurrentContext() {
		this.CurrentContextPrototype = new UserContextObject.Prototype();
	}


    public bool CanSaveCurrentContext() {
        return this.CurrentContextPrototype.IsValid();
    }
    
    private async Task Create_Async() {
        await this.UserContextsData.CreateForCurrentUser_Async(
            this.CurrentContextPrototype.ToRaw()
        );

        if( this.OnCreate_Async is not null ) {
            await this.OnCreate_Async.Invoke( this.CurrentContextPrototype! );
        }
    }
    
    private async Task Update_Async() {
        await this.UserContextsData.UpdateForCurrentUser_Async(
            this.CurrentContextPrototype.ToRaw()
        );
    }
}