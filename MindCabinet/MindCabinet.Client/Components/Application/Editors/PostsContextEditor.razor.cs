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
    public PostsContextObject? InitialContext { get; set; } = null;
    private PostsContextObject? CurrentInitialContext = null;


    [Parameter]
    public string? AddedClasses { get; set; } = null;
	private PostsContextObject.Prototype EditContext_Prototype = new PostsContextObject.Prototype();


    public delegate Task OnEntryEdit_Func( PostsContextTermEntryObject.Raw entry, bool isAdded );

    [Parameter]
    public OnEntryEdit_Func? OnEntryEdit_Async { get; set; } = null;

    public delegate Task OnUpdate_Func( PostsContextObject.Raw context );

    [Parameter]
    public OnUpdate_Func? OnUpdate_Async { get; set; } = null;



	protected override void OnParametersSet() {
		base.OnParametersSet();
        
//Console.WriteLine( $"PostsContextEditor.OnParametersSet: InitialContext: {JsonSerializer.Serialize(this.InitialContext)}" );
        if( this.CurrentInitialContext != this.InitialContext ) {
            this.ResetEditContextToInitialState();
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

        this.EditContext_Prototype.Entries = this.EditContext_Prototype.Entries
            .Append( contextTerm )
            .ToArray();

        return contextTerm;
	}

	private async Task<PostsContextTermEntryObject.Raw?> RemoveTag_Async( TermObject newTag ) {
        await Task.CompletedTask;
        
        List<PostsContextTermEntryObject.Raw> entries = this.EditContext_Prototype.Entries.ToList();
        PostsContextTermEntryObject.Raw? removed = null;

        for( int i = 0; i < entries.Count; i++ ) {
            if( entries[i].TermId == newTag.Id ) {
                removed = entries[i];

                entries.RemoveAt( i );
                break;
            }
        }

        this.EditContext_Prototype.Entries = entries.ToArray();

        return removed;
	}

    
	private (bool HasUnsavedChanges, PostsContextObject.Prototype.MatchResult MatchResult) HasUnsavedChanges() {
		if( !this.EditContext_Prototype.IsValid(true) ) {
			return (false, PostsContextObject.Prototype.MatchResult.Unknown);
		}
        if( this.InitialContext is null ) {
            return (this.EditContext_Prototype.Name is not null
                    || this.EditContext_Prototype.Description is not null
                    || this.EditContext_Prototype.Entries.Any(),
                PostsContextObject.Prototype.MatchResult.Unknown
            );
        }

        // ClientDataAccess_PostsContext.Get_Return contexts = await this.PostsContextsData.GetForCurrentUserByCriteria_Async(
        //     new ClientDataAccess_PostsContext.GetForCurrentUserByCriteria_Params {
        //         Ids = [ this.InitialContext.Id ]
        //     }
        // );

        // PostsContextObject.Raw currentContextRaw = contexts
        //     .Contexts
        //     .First( ctx => ctx.Id == this.InitialContext.Id );
        // PostsContextObject currentContext = await ClientDataAccess_PostsContext.ToObject_Async(
        //     this.TermsData,
        //     currentContextRaw
        // );

		// return currentContext is not null
        //     ? !this.CurrentContextPrototype.Matches( currentContext!, ignoreId: true )
        //     : false;
        
		PostsContextObject.Prototype.MatchResult matchResult = this.EditContext_Prototype.Matches(
            this.InitialContext,
            ignoreId: true
        );

        return (matchResult != PostsContextObject.Prototype.MatchResult.Match, matchResult);
	}


    private void ClearEditContext() {
        this.InitialContext = null;
        this.ResetEditContextToInitialState();
    }

	private void ResetEditContextToInitialState() {
		this.EditContext_Prototype = this.InitialContext is not null
            ? this.InitialContext.ToPrototype()
            : new PostsContextObject.Prototype();
            
        this.CurrentInitialContext = this.InitialContext;
	}
    

    private async Task UpdateOrCreate_Async() {
        bool isUpdate = this.EditContext_Prototype.Id is not null;
        PostsContextObject.Raw raw = this.EditContext_Prototype.ToRaw( validateId: isUpdate );
        
        if( !isUpdate ) {
            raw.Id = (await this.PostsContextsData.CreateForCurrentUser_Async( raw )).Id;
        } else {
            await this.PostsContextsData.UpdateForCurrentUser_Async( this.EditContext_Prototype );
        }

        this.InitialContext = await ClientDataAccess_PostsContext.ConvertRawToDataObject_Async(
            termsData: this.TermsData,
            ctxRaw: raw
        );
        this.ResetEditContextToInitialState();

        if( this.OnUpdate_Async is not null ) {
            await this.OnUpdate_Async.Invoke( raw );
        }
    }
}