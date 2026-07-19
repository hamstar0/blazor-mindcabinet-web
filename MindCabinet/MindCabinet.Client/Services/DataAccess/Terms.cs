using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.Utility;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_Terms : IClientDataAccess {
    private static readonly SimpleCache<TermId, TermObject.Raw?> Cache_ById = new( refreshExpiryOnGet: true );


    
    private HttpClient Http;



    public ClientDataAccess_Terms( HttpClient http ) {
        this.Http = http;
    }


    public async Task<IAPI.GetByX_Return> GetByIds_Async( IEnumerable<TermId> termIds ) {
        IEnumerable<TermObject.Raw?> terms = Cache_ById.GetMany( termIds );
        if( terms.Count() == termIds.Count() ) {
            return new IAPI.GetByX_Return( terms.Select(t => t!) );
        }

        //

        var ret = await IClientDataAccess.CallAPI_Async<IEnumerable<TermId>, IAPI.GetByX_Return>(
            http: this.Http,
            route: $"{IAPI.BaseRoute}/{nameof(IAPI.GetByIds_Async)}",
            parameters: termIds
        );

        //

        foreach( TermObject.Raw term in ret.Terms ) {
            Cache_ById.Set( term.Id, term, TimeSpan.FromDays(365) );
        }

        //
        
        return ret;
    }


    public async Task<IAPI.GetByX_Return> GetByCriteria_Async( IAPI.GetByCriteria_Params parameters ) {
        var ret = await IClientDataAccess.CallAPI_Async<IAPI.GetByCriteria_Params, IAPI.GetByX_Return>(
            http: this.Http,
            route: $"{IAPI.BaseRoute}/{nameof(IAPI.GetByCriteria_Async)}",
            parameters: parameters
        );
        
        return ret;
    }


    public async Task<IAPI.Create_Return> Create_Async( IAPI.Create_Params parameters ) {
        var ret = await IClientDataAccess.CallAPI_Async<IAPI.Create_Params, IAPI.Create_Return>(
            http: this.Http,
            route: $"{IAPI.BaseRoute}/{nameof(IAPI.CreateForCurrentUser_Async)}",
            parameters: parameters
        );

        //

        Cache_ById.Set( key: ret.TermRaw.Id, ret.TermRaw, TimeSpan.FromDays(365) );

        //

        return ret;
    }
}
