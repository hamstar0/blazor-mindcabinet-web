using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserPostsContext;


namespace MindCabinet.Client.Components.Application.Editors;


public partial class UserPostsContextEditor : ComponentBase {
    //[Inject]
    //private IJSRuntime Js { get; set; } = null!;

    //[Inject]
    //private ClientDbAccess DbAccess { get; set; } = null!;

    [Inject]
    private ClientDataAccess_Terms TermsData { get; set; } = null!;

    [Inject]
    private ClientDataAccess_UserPostsContext UserPostsContextsData { get; set; } = null!;

    [Inject]
    private ClientSessionData SessionsData { get; set; } = null!;


    [Parameter]
    public string? AddedClasses { get; set; } = null;

    [Parameter]
    public UserPostsContextObject? InitialContext { get; set; } = null;

    private UserPostsContextObject? InitialContextCheck;

	private UserPostsContextObject.Prototype CurrentContextPrototype = new UserPostsContextObject.Prototype();

    [Parameter]
    public Func<UserPostsContextObject.Prototype, Task>? OnCreate_Async { get; set; } = null;



	protected override void OnInitialized() {
		base.OnInitialized();
        
        if( this.InitialContext is null ) {
            this.InitialContextCheck = this.InitialContext;
            this.CurrentContextPrototype = new UserPostsContextObject.Prototype();
        }
	}
    
	protected override void OnParametersSet() {
		base.OnParametersSet();
        
        if( this.InitialContext != this.InitialContextCheck ) {
            this.InitialContextCheck = this.InitialContext;
            
            if( this.InitialContext is not null ) {
                this.CurrentContextPrototype = new UserPostsContextObject.Prototype {
                    Name = this.InitialContext?.Name,
                    Description = this.InitialContext?.Description,
                    Entries = this.InitialContext?.Entries
                        .Select( e => e.ToRaw(this.InitialContext.Id) ).ToArray()
                        ?? []
                };
            } else {
                this.CurrentContextPrototype = new UserPostsContextObject.Prototype();
            }
        }
	}


	private async Task AddNewTag_Async( TermObject newTag ) {
        this.CurrentContextPrototype.Entries.Append( new UserPostsContextTermEntryObject.Raw {
            TermId = newTag.Id,
            Priority = 0d,
            IsRequired = false
        } );
	}

	private async Task RemoveTag_Async( TermObject newTag ) {
        List<UserPostsContextTermEntryObject.Raw> entries = this.CurrentContextPrototype.Entries.ToList();

        for( int i = 0; i < entries.Count; i++ ) {
            if( entries[i].TermId == newTag.Id ) {
                entries.RemoveAt( i );
                break;
            }
        }

        this.CurrentContextPrototype.Entries = entries.ToArray();
	}

    
	private async Task<bool> HasUnsavedChanges() {
        UserPostsContextObject? currentCtx = this.SessionsData.GetCurrentContext();
        
        if( currentCtx is null ) {
            return this.CurrentContextPrototype.Name is not null
                || this.CurrentContextPrototype.Description is not null
                || this.CurrentContextPrototype.Entries.Any();
        }

        ClientDataAccess_UserPostsContext.Get_Return contexts = await this.UserPostsContextsData.GetForCurrentUserByCriteria_Async(
            new ClientDataAccess_UserPostsContext.GetForCurrentUserByCriteria_Params {
                Ids = [ currentCtx.Id ]
            }
        );

        UserPostsContextObject.Raw currentContextRaw = contexts
            .Contexts
            .First( ctx => ctx.Id == currentCtx.Id );
        UserPostsContextObject currentContext = await ClientDataAccess_UserPostsContext.ToObject_Async( this.TermsData, currentContextRaw );

		return currentContext is not null
            ? !this.CurrentContextPrototype.Matches( currentContext! )
            : false;
	}


	private void ResetCurrentContext() {
		this.CurrentContextPrototype = new UserPostsContextObject.Prototype();
	}


    public bool CanSaveCurrentContext() {
        return this.CurrentContextPrototype.IsValid();
    }
    
    private async Task Create_Async() {
        await this.UserPostsContextsData.CreateForCurrentUser_Async(
            this.CurrentContextPrototype.ToRaw()
        );

        if( this.OnCreate_Async is not null ) {
            await this.OnCreate_Async.Invoke( this.CurrentContextPrototype! );
        }
    }
    
    private async Task Update_Async() {
        await this.UserPostsContextsData.UpdateForCurrentUser_Async(
            this.CurrentContextPrototype.ToRaw()
        );
    }
}