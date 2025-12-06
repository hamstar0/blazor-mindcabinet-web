using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserContext;


namespace MindCabinet.Client.Components.Application.Editors;


public partial class CurrentContextEditor : ComponentBase {
    //[Inject]
    //private IJSRuntime Js { get; set; } = null!;

    //[Inject]
    //private ClientDbAccess DbAccess { get; set; } = null!;

    [Inject]
    private ClientDataAccess_Terms TermsData { get; set; } = null!;


    [Parameter]
    public string? AddedClasses { get; set; } = null;

	[Parameter, EditorRequired]
	public List<UserContextObject> Contexts { get; set; } = null!;

	[Parameter]
	public UserContextObject? CurrentContext { get; set; } = null;

    [Parameter]
    public Func<List<TermObject>, Task>? OnSubmit_Async { get; set; } = null;


	
	private async Task AddNewTag_Async( TermObject newTag ) {
		if( this.CurrentContext is null ) {
			throw new Exception( "CurrentContext is null" );
		}

        this.CurrentContext.Entries.Add( new UserContextEntryObject(newTag, 0d, false) );
	}

	private async Task RemoveTag_Async( TermObject newTag ) {
		if( this.CurrentContext is null ) {
			throw new Exception( "CurrentContext is null" );
		}

        for( int i = 0; i < this.CurrentContext.Entries.Count; i++ ) {
            if( this.CurrentContext.Entries[i].Term.Id == newTag.Id ) {
                this.CurrentContext.Entries.RemoveAt( i );
                break;
            }
        }
	}


    public bool CanSubmit() {
    }
    
    private async Task Submit_UI_Async() {
        add to database

        if( this.OnSubmit_Async is not null ) {
            await this.OnSubmit_Async.Invoke( this.Tags );
        }
    }
}