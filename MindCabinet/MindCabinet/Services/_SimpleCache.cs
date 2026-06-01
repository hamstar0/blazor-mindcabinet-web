using System.Collections.Concurrent;
using System.Data;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Components;
using MindCabinet.Data;
using MindCabinet.Data.DataAccess;
using MindCabinet.Extensions;
using MindCabinet.Services;
using MindCabinet.Shared.Utility;


namespace MindCabinet.Services;



public class SimpleCache {
    private readonly ConcurrentDictionary<string, (string data, DateTime expires)> _Cache = new();



    public void Set( string key, string data, TimeSpan expiry ) {
        this._Cache[ key ] = (data, DateTime.UtcNow.Add(expiry));
    }

    public bool TryGet( string key, out string data ) {
        if( this._Cache.TryGetValue(key, out var entry) ) {
            if( entry.expires > DateTime.UtcNow ) {
                data = entry.data;
                return true;
            }
            this._Cache.TryRemove( key, out _ );
        }
        data = default!;
        return false;
    }
}
