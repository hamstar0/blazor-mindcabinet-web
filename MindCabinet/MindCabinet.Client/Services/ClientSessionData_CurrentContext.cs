using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserContext;
using System.Net.Http.Json;

namespace MindCabinet.Client.Services;


public partial class ClientSessionData {
    private long? CurrentContextId;


    public long? GetCurrentContextById() {
        return this.CurrentContextId;
    }

    public void SetCurrentContextById( long contextId ) {
        this.CurrentContextId = contextId;
    }
}
