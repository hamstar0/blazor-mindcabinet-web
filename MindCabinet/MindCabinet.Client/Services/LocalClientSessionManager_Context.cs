using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsContext;
using System.Net.Http.Json;

namespace MindCabinet.Client.Services;


public partial class LocalClientSessionManager {
    public PostsContextObject? GetCurrentContext() {
        return this.Data?.UserAppData?.CurrentPostsContext;
    }


    public async Task SetCurrentContext_Await( ClientDataAccess_UserAppData userAppDataSrc, PostsContextObject context ) {
        if( this.Data?.UserAppData is null ) {
            throw new InvalidOperationException( "UserAppData is null in SetCurrentContext." );
        }

        await userAppDataSrc.UpdateForCurrentUser_Async( new UserAppDataObject.Prototype {
            SimpleUserId = this.UserId,
            CurrentPostsContextId = context.Id
        } );
        this.Data.UserAppData.SetCurrentPostsContext( context );
        
        await this.TriggerPostsContextChanged_Async( context );
    }
}
