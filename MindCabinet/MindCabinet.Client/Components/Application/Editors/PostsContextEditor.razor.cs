using System.Text.Json;
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


    [Parameter]
    public string? AddedClasses { get; set; } = null;

    [Parameter]
    public UserPostsContextObject? InitialContext { get; set; } = null;

    private UserPostsContextObject? InitialContextCheck;

	private UserPostsContextObject.Prototype CurrentContextPrototype = new UserPostsContextObject.Prototype();


    public delegate Task OnEntryEdit_Func( UserPostsContextTermEntryObject.Raw entry, bool isAdded );

    [Parameter]
    public OnEntryEdit_Func? OnEntryEdit_Async { get; set; } = null;

    public delegate Task OnCreateOrUpdate_Func( UserPostsContextObject.Raw context );

    [Parameter]
    public OnCreateOrUpdate_Func? OnCreateOrUpdate_Async { get; set; } = null;



	protected override void OnParametersSet() {
		base.OnParametersSet();
        
        if( this.InitialContext != this.InitialContextCheck ) {
            this.InitialContextCheck = this.InitialContext;
            
            this.CurrentContextPrototype = this.InitialContext is not null
                ? this.InitialContext.ToPrototype()
                : new UserPostsContextObject.Prototype();
        }
	}


	private async Task<UserPostsContextTermEntryObject.Raw> AddNewTag_Async( TermObject newTag ) {
        await Task.CompletedTask;
        
        UserPostsContextId id = this.InitialContext?.Id
            ?? throw new InvalidOperationException("Invalid prototype id.");

        var contextTerm = UserPostsContextTermEntryObject.CreateRaw(
            userPostsContextId: id,
            termId: newTag.Id,
            priority: 0d,
            isRequired: false
        );

        this.CurrentContextPrototype.Entries.Append( contextTerm );

        return contextTerm;
	}

	private async Task<UserPostsContextTermEntryObject.Raw?> RemoveTag_Async( TermObject newTag ) {
        await Task.CompletedTask;
        
        List<UserPostsContextTermEntryObject.Raw> entries = this.CurrentContextPrototype.Entries.ToList();
        UserPostsContextTermEntryObject.Raw? removed = null;

        for( int i = 0; i < entries.Count; i++ ) {
            if( entries[i].TermId == newTag.Id ) {
                removed = entries[i];

                entries.RemoveAt( i );
                break;
            }
        }

        this.CurrentContextPrototype.Entries = entries.ToArray();

        return removed;
	}

    
	private async Task<bool> HasUnsavedChanges() {
        if( this.InitialContext is null ) {
            return this.CurrentContextPrototype.Name is not null
                || this.CurrentContextPrototype.Description is not null
                || this.CurrentContextPrototype.Entries.Any();
        }

        ClientDataAccess_UserPostsContext.Get_Return contexts = await this.UserPostsContextsData.GetForCurrentUserByCriteria_Async(
            new ClientDataAccess_UserPostsContext.GetForCurrentUserByCriteria_Params {
                Ids = [ this.InitialContext.Id ]
            }
        );

        UserPostsContextObject.Raw currentContextRaw = contexts
            .Contexts
            .First( ctx => ctx.Id == this.InitialContext.Id );
        UserPostsContextObject currentContext = await ClientDataAccess_UserPostsContext.ToObject_Async( this.TermsData, currentContextRaw );

		return currentContext is not null
            ? !this.CurrentContextPrototype.Matches( currentContext! )
            : false;
	}


	private void ResetCurrentContext() {
		this.CurrentContextPrototype = new UserPostsContextObject.Prototype();
	}


    private async Task Create_Async() {
        UserPostsContextObject.Raw raw = this.CurrentContextPrototype.ToRaw(false);

        UserPostsContextId id = (await this.UserPostsContextsData.CreateForCurrentUser_Async( raw )).Id;

        raw.Id = id;
        this.CurrentContextPrototype.Id = id;

        if( this.OnCreateOrUpdate_Async is not null ) {
            await this.OnCreateOrUpdate_Async.Invoke( raw );
        }
    }
    
    private async Task Update_Async() {
        await this.UserPostsContextsData.UpdateForCurrentUser_Async( this.CurrentContextPrototype );

        UserPostsContextObject.Raw raw = this.CurrentContextPrototype.ToRaw(true);

        if( this.OnCreateOrUpdate_Async is not null ) {
            await this.OnCreateOrUpdate_Async.Invoke( raw );
        }
    }
}