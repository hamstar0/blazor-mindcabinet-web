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


    [Parameter]
    public string? AddedClasses { get; set; } = null;

	[Parameter]
	public UserContextObject? CurrentContext { get; set; } = null;
    
	private UserContextObject.Prototype CurrentContextPrototype = new UserContextObject.Prototype();

    [Parameter]
    public Func<UserContextObject, Task>? OnCreate_Async { get; set; } = null;


	
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

    
    public bool CanCreateNewContext() {
    }
    
    private async Task Create_Async() {
        add to database

        if( this.OnCreate_Async is not null ) {
            await this.OnCreate_Async.Invoke( this.CurrentContext! );
        }
    }
}