using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserContext;
using System.Net.Http.Json;

namespace MindCabinet.Client.Services;


public partial class ClientSessionData {
    private IEnumerable<UserContextObject> CurrentContexts_Cache = new List<UserContextObject>();

    private long CurrentContextId;



    private async Task LoadContexts_Async() {
        this.CurrentContexts_Cache = await this.UserContextData.GetForCurrentUserByCriteria_Async(
            new ClientDataAccess_UserContext.GetForCurrentUserByCriteria_Params( null )
        );
    }


    public List<UserContextObject> GetUserContexts() {
        return this.CurrentContexts_Cache.ToList();
    }

    public UserContextObject? GetCurrentUserContext() {
        if( this.CurrentContextId == 0 ) {
            return null;
        }

        return this.CurrentContexts_Cache.FirstOrDefault( c => c.Id == this.CurrentContextId );
    }

    public void SetCurrentContextById( long contextId ) {
        this.CurrentContextId = contextId;
    }
}
