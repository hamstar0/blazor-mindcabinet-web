using System.Net.Http.Json;
using System.Text.Json;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.PostsContext;
using MindCabinet.Shared.DataObjects.Term;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Components;
using MindCabinet.Shared.Utility;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_PostsContext : IClientDataAccess {
    private static readonly SimpleCache<PostsContextId, PostsContextObject.Raw?> Cache_ById = new( refreshOnGet: true );


    
    // private HttpClient Http;

    private LocalClientSessionManager MySessionMngr;

    private HubConnection HubConnection;



    public ClientDataAccess_PostsContext( LocalClientSessionManager mySessionMngr, NavigationManager navigationManager ) {
        this.MySessionMngr = mySessionMngr;

        Uri hubUrl = navigationManager.ToAbsoluteUri( IAPI.BaseRoute );
        this.HubConnection = new HubConnectionBuilder()
            .WithUrl( hubUrl )
            .Build();
    }

    public async ValueTask DisposeAsync() {
        await this.HubConnection.DisposeAsync();
    }


    public async Task<IAPI.Get_Return> GetForCurrentUserByCriteria_Async(
                IAPI.GetByCriteria_Params parameters ) {
        if( this.MySessionMngr.UserId is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        //

        IEnumerable<PostsContextObject.Raw?> contexts = Cache_ById.GetMany( parameters.Ids );
        if( contexts.Count() == parameters.Ids.Length ) {
            return new IAPI.Get_Return {
                Contexts = contexts.Select( c => c! )
            };
        }

        //

        IAPI.Get_Return ret = await IClientDataAccess.CallHub_Async<IAPI.Get_Return>(
            hubConnection: this.HubConnection,
            methodName: nameof( IAPI.GetForCurrentUserByCriteria_Async ),
            args: new object[] { parameters }
        );

        //

        foreach( PostsContextObject.Raw ctx in ret.Contexts ) {
            Cache_ById.Set( ctx.Id, ctx, TimeSpan.FromDays(365) );
        }

        //

        return ret;
    }


    public async Task<IAPI.CreateOrUpdate_Return> CreateForCurrentUser_Async( PostsContextObject.Raw parameters ) {
        if( this.MySessionMngr.UserId is null ) {
            throw new InvalidOperationException( "No user in session" );
        }
        if( !parameters.IsValid(true) ) {
            throw new ArgumentException( $"Invalid PostsContextObject.Raw parameter: {JsonSerializer.Serialize(parameters)}" );
        }

        IAPI.CreateOrUpdate_Return ret = await IClientDataAccess.CallHub_Async<IAPI.CreateOrUpdate_Return>(
            hubConnection: this.HubConnection,
            methodName: nameof( IAPI.CreateForCurrentUser_Async ),
            args: new object[] { parameters }
        );

        //

        Cache_ById.Set( parameters.Id, parameters, TimeSpan.FromDays(365) );

        //

        return ret;
    }
    

    public async Task<IAPI.CreateOrUpdate_Return> UpdateForCurrentUser_Async( PostsContextObject.Prototype parameters ) {
        if( this.MySessionMngr.UserId is null ) {
            throw new InvalidOperationException( "No user in session" );
        }
        if( parameters.Id is null || parameters.Id == 0 ) {
            throw new ArgumentException( "PostsContextObject.Prototype Id is not valid (must be non-zero and non-null)." );
        }

        //

        Cache_ById.Remove( parameters.Id.Value );

        //

        return await IClientDataAccess.CallHub_Async<IAPI.CreateOrUpdate_Return>(
            hubConnection: this.HubConnection,
            methodName: nameof( IAPI.UpdateForCurrentUser_Async ),
            args: new object[] { parameters }
        );
    }
}
