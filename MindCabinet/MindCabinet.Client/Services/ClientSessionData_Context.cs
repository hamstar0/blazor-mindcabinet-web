using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserContext;
using System.Net.Http.Json;

namespace MindCabinet.Client.Services;


public partial class ClientSessionData {
    public UserContextObject? GetCurrentContext() {
        return this.Data?.UserAppData?.UserContext;
    }


    public async Task SetCurrentContext_Await( UserContextObject context ) {
        if( this.Data?.UserAppData is null ) {
            throw new InvalidOperationException( "UserAppData is null in SetCurrentContext." );
        }

        this.Data.UserAppData.SetUserContext( context );
        
        await Task.WhenAll(
            this.OnUserContextChanged_Async
                .Select( f => f.Invoke(context) )
        );
    }
}
