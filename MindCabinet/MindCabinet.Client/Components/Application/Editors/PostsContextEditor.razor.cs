using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsContext;


namespace MindCabinet.Client.Components.Application.Editors;


public partial class PostsContextEditor : ComponentBase {
    //[Inject]
    //private IJSRuntime Js { get; set; } = null!;

    //[Inject]
    //private ClientDbAccess DbAccess { get; set; } = null!;

    [Inject]
    private ClientDataAccess_Terms TermsData { get; set; } = null!;

    [Inject]
    private ClientDataAccess_PostsContext PostsContextsData { get; set; } = null!;


    [Parameter]
    public string? AddedClasses { get; set; } = null;

    [Parameter]
    public PostsContextObject? InitialContext { get; set; } = null;

    private bool IsInitialized;

	private PostsContextObject.Prototype CurrentContextPrototype = new PostsContextObject.Prototype();


    public delegate Task OnEntryEdit_Func( PostsContextTermEntryObject.Raw entry, bool isAdded );

    [Parameter]
    public OnEntryEdit_Func? OnEntryEdit_Async { get; set; } = null;

    public delegate Task OnCreateOrUpdate_Func( PostsContextObject.Raw context );

    [Parameter]
    public OnCreateOrUpdate_Func? OnCreateOrUpdate_Async { get; set; } = null;



	protected override void OnParametersSet() {
		base.OnParametersSet();
        
//Console.WriteLine( $"PostsContextEditor.OnParametersSet: InitialContext: {JsonSerializer.Serialize(this.InitialContext)}" );
        if( !this.IsInitialized && this.InitialContext is not null ) {
            this.IsInitialized = true;
            
            this.CurrentContextPrototype = this.InitialContext.ToPrototype();
        }
	}


	private async Task<PostsContextTermEntryObject.Raw> AddNewTag_Async( TermObject newTag ) {
        await Task.CompletedTask;
        
        PostsContextId id = this.InitialContext?.Id
            ?? throw new InvalidOperationException("Invalid prototype id.");

        var contextTerm = PostsContextTermEntryObject.CreateRaw(
            postsContextId: id,
            termId: newTag.Id,
            priority: 0d,
            isRequired: false
        );

        this.CurrentContextPrototype.Entries = this.CurrentContextPrototype.Entries
            .Append( contextTerm )
            .ToArray();

        return contextTerm;
	}

	private async Task<PostsContextTermEntryObject.Raw?> RemoveTag_Async( TermObject newTag ) {
        await Task.CompletedTask;
        
        List<PostsContextTermEntryObject.Raw> entries = this.CurrentContextPrototype.Entries.ToList();
        PostsContextTermEntryObject.Raw? removed = null;

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

        ClientDataAccess_PostsContext.Get_Return contexts = await this.PostsContextsData.GetForCurrentUserByCriteria_Async(
            new ClientDataAccess_PostsContext.GetForCurrentUserByCriteria_Params {
                Ids = [ this.InitialContext.Id ]
            }
        );

        PostsContextObject.Raw currentContextRaw = contexts
            .Contexts
            .First( ctx => ctx.Id == this.InitialContext.Id );
        PostsContextObject currentContext = await ClientDataAccess_PostsContext.ToObject_Async( this.TermsData, currentContextRaw );

		return currentContext is not null
            ? !this.CurrentContextPrototype.Matches( currentContext! )
            : false;
	}


	private void ResetCurrentContext() {
		this.CurrentContextPrototype = new PostsContextObject.Prototype();
	}


    private async Task Create_Async() {
        PostsContextObject.Raw raw = this.CurrentContextPrototype.ToRaw(false);

        PostsContextId id = (await this.PostsContextsData.CreateForCurrentUser_Async( raw )).Id;

        raw.Id = id;
        this.CurrentContextPrototype.Id = id;

        if( this.OnCreateOrUpdate_Async is not null ) {
            await this.OnCreateOrUpdate_Async.Invoke( raw );
        }
    }
    
    private async Task Update_Async() {
        await this.PostsContextsData.UpdateForCurrentUser_Async( this.CurrentContextPrototype );

        PostsContextObject.Raw raw = this.CurrentContextPrototype.ToRaw( true );

        if( this.OnCreateOrUpdate_Async is not null ) {
            await this.OnCreateOrUpdate_Async.Invoke( raw );
        }
    }
}