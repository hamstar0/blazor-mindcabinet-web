using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserPostsContext;
using System.Net.Http.Json;

namespace MindCabinet.Client.Services;


public partial class ClientSessionData {
    public UserPostsContextObject? GetCurrentContext() {
        return this.Data?.UserAppData?.UserPostsContext;
    }


    public async Task SetCurrentContext_Await( UserPostsContextObject context ) {
        if( this.Data?.UserAppData is null ) {
            throw new InvalidOperationException( "UserAppData is null in SetCurrentContext." );
        }

        this.Data.UserAppData.SetUserPostsContext( context );
        
        await this.TriggerUserPostsContextChanged_Async( context );
    }
}
