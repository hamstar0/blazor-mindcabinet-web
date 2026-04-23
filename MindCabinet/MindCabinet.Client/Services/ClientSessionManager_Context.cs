using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsContext;
using System.Net.Http.Json;

namespace MindCabinet.Client.Services;


public partial class ClientSessionManager {
    public PostsContextObject? GetCurrentContext() {
        return this.Data?.UserAppData?.PostsContext;
    }


    public async Task SetCurrentContext_Await( ClientDataAccess_UserAppData userAppData, PostsContextObject context ) {
        if( this.Data?.UserAppData is null ) {
            throw new InvalidOperationException( "UserAppData is null in SetCurrentContext." );
        }

        await userAppData.UpdateForCurrentUser_Async( new UserAppDataObject.Prototype {
            SimpleUserId = this.UserId,
            PostsContextId = context.Id
        } );
        this.Data.UserAppData.SetPostsContext( context );
        
        await this.TriggerPostsContextChanged_Async( context );
    }
}
