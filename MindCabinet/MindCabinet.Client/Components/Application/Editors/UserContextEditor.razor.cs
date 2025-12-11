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
                    .Select( e => new UserContextEntryObject.DatabaseEntry {
                        TermId = e.Term.Id,
                        Priority = e.Priority,
                        IsRequired = e.IsRequired
                    } ).ToList()
                    ?? new List<UserContextEntryObject.DatabaseEntry>()
            };
        }
	}


	private async Task AddNewTag_Async( TermObject newTag ) {
        this.CurrentContextPrototype.Entries.Add( new UserContextEntryObject.DatabaseEntry {
            TermId = newTag.Id,
            Priority = 0d,
            IsRequired = false
        } );
	}

	private async Task RemoveTag_Async( TermObject newTag ) {
        for( int i = 0; i < this.CurrentContextPrototype.Entries.Count; i++ ) {
            if( this.CurrentContextPrototype.Entries[i].TermId == newTag.Id ) {
                this.CurrentContextPrototype.Entries.RemoveAt( i );
                break;
            }
        }
	}

    
	private async Task<bool> HasUnsavedChanges() {
        long? currentContextId = this.SessionsData.GetCurrentContextById();
        
        if( currentContextId is null ) {
            return this.CurrentContextPrototype.Name is not null
                || this.CurrentContextPrototype.Description is not null
                || this.CurrentContextPrototype.Entries.Any();
        }

        IEnumerable<UserContextObject> contexts = await this.UserContextsData.GetForCurrentUserByCriteria_Async(
            new ClientDataAccess_UserContext.GetForCurrentUserByCriteria_Params {
                Ids = [ currentContextId.Value ]
            }
        );

        UserContextObject currentContext = contexts.First( ctx => ctx.Id == currentContextId );

		return !this.CurrentContextPrototype
			.Matches( currentContext );
	}


	private void ResetCurrentContext() {
		this.CurrentContextPrototype = new UserContextObject.Prototype();
	}


    public bool CanSaveCurrentContext() {
        return this.CurrentContextPrototype.IsValid();
    }
    
    private async Task Create_Async() {
        await this.UserContextsData.CreateForCurrentUser_Async(
            this.CurrentContextPrototype.CreateDatabaseEntry()
        );

        if( this.OnCreate_Async is not null ) {
            await this.OnCreate_Async.Invoke( this.CurrentContextPrototype! );
        }
    }
    
    private async Task Update_Async() {
        await this.UserContextsData.UpdateForCurrentUser_Async(
            this.CurrentContextPrototype.CreateDatabaseEntry()
        );
    }
}