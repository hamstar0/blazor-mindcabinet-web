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
	
    
    private PostsContextId? EditContext_Id;

    private string? EditContext_Name;
    
    private string? EditContext_Description;

    private List<PostsContextTermEntryObject> EditContext_Entries = [];


    [Parameter]
    public string? AddedClasses { get; set; } = null;


    public delegate Task OnEntryEdit_Func( PostsContextTermEntryObject entry, bool isAdded );

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


	private PostsContextTermEntryObject AddNewTag( TermObject newTag ) {
        PostsContextId id = this.InitialContext?.Id
            ?? throw new InvalidOperationException("Invalid prototype id.");
        
        var contextTerm = new PostsContextTermEntryObject(
            term: newTag,
            priority: 0d,
            isRequired: false
        );

        this.EditContext_Entries.Add( contextTerm );

        return contextTerm;
	}

	private PostsContextTermEntryObject RemoveTag( TermObject newTag ) {
        PostsContextTermEntryObject entryToRemove = this.GetFirstMatchingTagEntry( newTag.Id );

        this.EditContext_Entries.Remove( entryToRemove );
        
        return entryToRemove;
	}

    
	private bool HasUnsavedChanges() {
        if( this.InitialContext is null ) {
            return this.EditContext_Name is not null
                    || this.EditContext_Description is not null
                    || this.EditContext_Entries.Any();
        }
		if( this.EditContext_Id is null || this.EditContext_Id == 0 ) {
			return false;
		}
        
		return this.InitialContext.Matches(
            id: this.InitialContext?.Id,
            name: this.InitialContext?.Name ?? "",
            description: this.InitialContext?.Description,
            entries: this.InitialContext?.Entries ?? []
        ) == PostsContextObject.MatchResult.Match;
	}


    private void ClearEditContext() {
        this.InitialContext = null;
        this.ResetEditContextToInitialState();
    }

	private void ResetEditContextToInitialState() {
        if( this.InitialContext is null ) {
            this.EditContext_Id = null;
            this.EditContext_Name = null;
            this.EditContext_Description = null;
            this.EditContext_Entries = [];
        } else {
            this.EditContext_Id = this.InitialContext.Id;
            this.EditContext_Name = this.InitialContext.Name;
            this.EditContext_Description = this.InitialContext.Description;
            this.EditContext_Entries = this.InitialContext.Entries.ToList();
        }
            
        this.CurrentInitialContext = this.InitialContext;
	}
    

    private async Task UpdateOrCreate_Async() {
        bool isUpdate = this.EditContext_Id is not null;
        PostsContextObject.Raw raw = PostsContextObject.CreateRaw(
            id: this.EditContext_Id ?? 0,
            name: this.EditContext_Name ?? "",
            description: this.EditContext_Description,
            entries: this.EditContext_Entries.Select( e => e.ToRaw(this.EditContext_Id ?? 0) ).ToArray()
        );
        
        if( !isUpdate ) {
            raw.Id = (await this.PostsContextsData.CreateForCurrentUser_Async( raw )).Id;
        } else {
            await this.PostsContextsData.UpdateForCurrentUser_Async( raw.ToPrototype() );
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
