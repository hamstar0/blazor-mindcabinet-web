using System.Net.Http.Json;
using System.Text.Json;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.PostsQuery;
using MindCabinet.Shared.DataObjects.Term;
using Microsoft.AspNetCore.Components;
using MindCabinet.Shared.Utility;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_PostsQuery : IClientDataAccess {
    private static readonly SimpleCache<PostsQueryId, PostsQueryObject.Raw?> Cache_ById = new( refreshExpiryOnGet: true );


    
    private HttpClient Http;

    private LocalClientSessionManager MySessionMngr;



    public ClientDataAccess_PostsQuery( HttpClient http, LocalClientSessionManager mySessionMngr ) {
        this.MySessionMngr = mySessionMngr;
        this.Http = http;
    }


    public async Task<PostsQueryObject.Raw?> GetForCurrentUserById_Async( PostsQueryId id ) {
        return (await this.GetForCurrentUserByCriteria_Async(
            new IAPI.GetByCriteria_Params { Ids = [id] }
        )).Queries.FirstOrDefault();
    }

    public async Task<IAPI.Get_Return> GetForCurrentUserByCriteria_Async(
                IAPI.GetByCriteria_Params parameters ) {
        if( this.MySessionMngr.UserId is null ) {
            throw new InvalidOperationException( "No user in session" );
        }

        //

        IAPI.Get_Return ret;

        IEnumerable<PostsQueryObject.Raw?> cached = Cache_ById.GetMany( parameters.Ids );
        if( parameters.Ids.Length > 0 && cached.Count() == parameters.Ids.Length ) {    // TODO optimize
            ret = new IAPI.Get_Return {
                Queries = cached.Select( c => c! )
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

        foreach( PostsQueryObject.Raw query in ret.Queries ) {
            Cache_ById.Set( query.Id, query, TimeSpan.FromDays(365) );
        }

        //

        return ret;
    }


    public async Task<IAPI.CreateOrUpdate_Return> CreateForCurrentUser_Async(
                PostsQueryObject.Prototype parameters ) {
        if( this.MySessionMngr.UserId is null ) {
            throw new InvalidOperationException( "No user in session" );
        }
        if( !parameters.IsValid(false) ) {
            throw new ArgumentException( $"Invalid PostsQueryObject.Prototype parameter: {JsonSerializer.Serialize(parameters)}" );
        }

        var ret = await IClientDataAccess.CallAPI_Async<PostsQueryObject.Prototype, IAPI.CreateOrUpdate_Return>(
            http: this.Http,
            route: $"{IAPI.BaseRoute}/{nameof(IAPI.CreateForCurrentUser_Async)}",
            parameters: parameters
        );
        PostsQueryId id = ret.Id;

        //

        parameters.Id = id;
        foreach( PostsQueryTermEntryObject.Prototype entry in parameters.Entries ) {
            entry.PostsQueryId = id;
        }

        Cache_ById.Set( id, parameters.ToRaw(true), TimeSpan.FromDays(365) );

        //

        return ret;
    }
    

    public async Task<IAPI.CreateOrUpdate_Return> UpdateForCurrentUser_Async(
                PostsQueryObject.Prototype parameters ) {
        if( this.MySessionMngr.UserId is null ) {
            throw new InvalidOperationException( "No user in session" );
        }
        if( parameters.Id is null || parameters.Id == 0 ) {
            throw new ArgumentException( "PostsQueryObject.Prototype Id is not valid (must be non-zero and non-null)." );
        }

        //

        var ret = await IClientDataAccess.CallAPI_Async<PostsQueryObject.Prototype, IAPI.CreateOrUpdate_Return>(
            http: this.Http,
            route: $"{IAPI.BaseRoute}/{nameof(IAPI.UpdateForCurrentUser_Async)}",
            parameters: parameters
        );

        //

        Cache_ById.Remove( parameters.Id.Value );

        return ret;
    }
}
