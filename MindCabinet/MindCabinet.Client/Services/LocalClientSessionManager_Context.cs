using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsQuery;
using System.Net.Http.Json;

namespace MindCabinet.Client.Services;


public partial class LocalClientSessionManager {
    public PostsQueryObject? GetCurrentContext() {
        return this.Data?.UserAppData?.CurrentPostsQuery;
    }


    public async Task SetCurrentContext_Await( ClientDataAccess_UserAppData userAppDataSrc, PostsQueryObject query ) {
        if( this.Data?.UserAppData is null ) {
            throw new InvalidOperationException( "UserAppData is null in SetCurrentContext." );
        }

        await userAppDataSrc.UpdateForCurrentUser_Async( new UserAppDataObject.Prototype {
            SimpleUserId = this.UserId,
            CurrentPostsQueryId = query.Id
        } );
        this.Data.UserAppData.SetCurrentPostsQuery( query );
        
        await this.TriggerPostsQueryChanged_Async( query );
    }
}
