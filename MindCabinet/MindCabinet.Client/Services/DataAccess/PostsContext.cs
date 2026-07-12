using System.Net.Http.Json;
using System.Text.Json;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.PostsContext;
using MindCabinet.Shared.DataObjects.Term;
using Microsoft.AspNetCore.Components;
using MindCabinet.Shared.Utility;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_PostsContext : IClientDataAccess {
    private static readonly SimpleCache<PostsContextId, PostsContextObject.Raw?> Cache_ById = new( refreshExpiryOnGet: true );


    
    private HttpClient Http;

    private LocalClientSessionManager MySessionMngr;



    public ClientDataAccess_PostsContext( HttpClient http, LocalClientSessionManager mySessionMngr ) {
        this.MySessionMngr = mySessionMngr;
        this.Http = http;
    }


    public async Task<IAPI.Get_Return> GetForCurrentUserByCriteria_Async(
                IAPI.GetByCriteria_Params parameters ) {
        if( this.MySessionMngr.UserId is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        //

        IAPI.Get_Return ret;

        IEnumerable<PostsContextObject.Raw?> cachedContexts = Cache_ById.GetMany( parameters.Ids );
        if( parameters.Ids.Length > 0 && cachedContexts.Count() == parameters.Ids.Length ) {
            ret = new IAPI.Get_Return {
                Contexts = cachedContexts.Select( c => c! )
            };
            return ret;
        }

        //

        ret = await IClientDataAccess.CallAPI_Async<IAPI.GetByCriteria_Params, IAPI.Get_Return>(
            http: this.Http,
            route: $"{IAPI.BaseRoute}/{nameof(IAPI.GetForCurrentUserByCriteria_Async)}",
            parameters: parameters
        );

        //

        foreach( PostsContextObject.Raw ctx in ret.Contexts ) {
            Cache_ById.Set( ctx.Id, ctx, TimeSpan.FromDays(365) );
        }

        //

        return ret;
    }


    public async Task<IAPI.CreateOrUpdate_Return> CreateForCurrentUser_Async(
                PostsContextObject.Raw parameters ) {
        if( this.MySessionMngr.UserId is null ) {
            throw new InvalidOperationException( "No user in session" );
        }
        if( !parameters.IsValid(true) ) {
            throw new ArgumentException( $"Invalid PostsContextObject.Raw parameter: {JsonSerializer.Serialize(parameters)}" );
        }

        var ret = await IClientDataAccess.CallAPI_Async<PostsContextObject.Raw, IAPI.CreateOrUpdate_Return>(
            http: this.Http,
            route: $"{IAPI.BaseRoute}/{nameof(IAPI.CreateForCurrentUser_Async)}",
            parameters: parameters
        );

        //

        Cache_ById.Set( parameters.Id, parameters, TimeSpan.FromDays(365) );

        //

        return ret;
    }
    

    public async Task<IAPI.CreateOrUpdate_Return> UpdateForCurrentUser_Async(
                PostsContextObject.Prototype parameters ) {
        if( this.MySessionMngr.UserId is null ) {
            throw new InvalidOperationException( "No user in session" );
        }
        if( parameters.Id is null || parameters.Id == 0 ) {
            throw new ArgumentException( "PostsContextObject.Prototype Id is not valid (must be non-zero and non-null)." );
        }

        //

        var ret = await IClientDataAccess.CallAPI_Async<PostsContextObject.Prototype, IAPI.CreateOrUpdate_Return>(
            http: this.Http,
            route: $"{IAPI.BaseRoute}/{nameof(IAPI.UpdateForCurrentUser_Async)}",
            parameters: parameters
        );

        //

        Cache_ById.Remove( parameters.Id.Value );

        return ret;
    }
}
