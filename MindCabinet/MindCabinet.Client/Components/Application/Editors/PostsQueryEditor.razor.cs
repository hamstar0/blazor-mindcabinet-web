using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsQuery;


namespace MindCabinet.Client.Components.Application.Editors;


public partial class PostsQueryEditor : ComponentBase {
    //[Inject]
    //private IJSRuntime Js { get; set; } = null!;

    [Inject]
    private LocalClientSessionManager SessionManager { get; set; } = null!;

    [Inject]
    private ClientDataAccess_Terms TermsDataSrc { get; set; } = null!;

    [Inject]
    private ClientDataAccess_PostsQuery PostsQueryDataSrc { get; set; } = null!;


    [Parameter]
    public PostsQueryObject? InitialContext { get; set; } = null;

    private PostsQueryObject? TemplateQuery = null;
	
    
    private PostsQueryId? EditQuery_Id;

    private string? EditQuery_Name;
    
    private string? EditQuery_Description;

    private List<PostsQueryTermEntryObject> EditQuery_Entries = [];


    [Parameter]
    public string? AddedClasses { get; set; } = null;


    public delegate Task OnEntryEdit_Func( PostsQueryTermEntryObject entry, bool isAdded );

    [Parameter]
    public OnEntryEdit_Func? OnEntryEdit_Async { get; set; } = null;

    public delegate Task OnUpdate_Func( PostsQueryObject.Raw query );

    [Parameter]
    public OnUpdate_Func? OnUpdate_Async { get; set; } = null;



	protected override void OnInitialized() {
		base.OnInitialized();
        
        this.TemplateQuery = this.InitialContext;
        this.ResetEditQueryToDefault();
	}


    public void SetDefaultQuery( PostsQueryObject? query ) {
        this.TemplateQuery = query;
        this.ResetEditQueryToDefault();
        
        this.StateHasChanged();
    }

	private PostsQueryTermEntryObject AddNewTag( TermObject newTag ) {
        var queryTerm = new PostsQueryTermEntryObject(
            term: newTag,
            priority: 0d,
            isRequired: false
        );

        this.EditQuery_Entries.Add( queryTerm );

        return queryTerm;
	}

	private PostsQueryTermEntryObject RemoveTag( TermObject newTag ) {
        PostsQueryTermEntryObject entryToRemove = this.GetFirstMatchingTagEntry( newTag.Id );

        this.EditQuery_Entries.Remove( entryToRemove );
        
        return entryToRemove;
	}

    
	public bool CanSaveEdits( bool ignoreId ) {
        if( !ignoreId && !PostsQueryObject.ValidateId(this.EditQuery_Id ?? default) ) {
            return false;
        }
        if( !PostsQueryObject.ValidateName(this.EditQuery_Name ?? "") ) {
            return false;
        }
        if( !PostsQueryObject.ValidateEntries(this.EditQuery_Entries.ToArray()) ) {
            return false;
        }

		return this.HasUnsavedChanges();
	}

	private bool HasUnsavedChanges() {
        if( this.TemplateQuery is null ) {
            return this.EditQuery_Id is not null
                    || this.EditQuery_Name is not null
                    || this.EditQuery_Description is not null
                    || this.EditQuery_Entries.Any();
        }
        
        if( this.EditQuery_Id != this.TemplateQuery.Id ) {
            throw new InvalidOperationException( $"Query ID mismatch: {this.EditQuery_Id} != {this.TemplateQuery.Id}" );
        }

		PostsQueryObject.MatchResult matchResult = this.TemplateQuery.Matches(
            id: this.EditQuery_Id,
            name: this.EditQuery_Name ?? "",
            description: this.EditQuery_Description,
            owner: this.TemplateQuery.Owner,
            entries: this.EditQuery_Entries.ToArray(),
            ignoreId: true
        );
        
        return matchResult != PostsQueryObject.MatchResult.Match;
	}


    private void ClearEditQuery() {
        this.TemplateQuery = null;
        this.ResetEditQueryToDefault();
    }

	private void ResetEditQueryToDefault() {
        if( this.TemplateQuery is null ) {
            this.EditQuery_Id = null;
            this.EditQuery_Name = null;
            this.EditQuery_Description = null;
            this.EditQuery_Entries = [];
        } else {
            this.EditQuery_Id = this.TemplateQuery.Id;
            this.EditQuery_Name = this.TemplateQuery.Name;
            this.EditQuery_Description = this.TemplateQuery.Description;
            this.EditQuery_Entries = this.TemplateQuery.Entries
                .Select( e => e.Clone() )
                .ToList();
        }
	}
    

    private async Task UpdateOrCreate_Async() {
        if( this.SessionManager.UserId is null || this.SessionManager.UserId == 0 ) {
            throw new Exception( "No user available. "+this.SessionManager.UserId );
        }

        bool isUpdate = this.EditQuery_Id is not null;

        PostsQueryObject.Raw raw = PostsQueryObject.CreateRaw(
            id: this.EditQuery_Id ?? 0,
            name: this.EditQuery_Name ?? "",
            description: this.EditQuery_Description,
            owner: this.SessionManager.UserId ?? throw new Exception("No user available."),
            entries: this.EditQuery_Entries
                .Select( e => e.ToRaw(this.EditQuery_Id ?? 0) )
                .ToArray()
        );
        
        if( !isUpdate ) {
            raw.Id = (await this.PostsQueryDataSrc.CreateForCurrentUser_Async( raw.ToPrototype() )).Id;
        } else {
            await this.PostsQueryDataSrc.UpdateForCurrentUser_Async( raw.ToPrototype() );
        }

        this.TemplateQuery = await ClientDataAccess_PostsQuery.ConvertRawToDataObject_Async(
            termsDataSrc: this.TermsDataSrc,
            queryRaw: raw
        );

        if( this.OnUpdate_Async is not null ) {
            await this.OnUpdate_Async.Invoke( raw );
        }
    }
}
