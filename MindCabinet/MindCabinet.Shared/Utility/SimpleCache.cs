using System.Collections.Concurrent;
using System.Data;
using MindCabinet.Shared.Utility;


namespace MindCabinet.Shared.Utility;



public class SimpleCache<TKey, TValue> 
                where TKey : notnull {
    private readonly ConcurrentDictionary<TKey, (TValue value, DateTime expires)> _Cache = new();



    public void Set( TKey key, TValue value, TimeSpan expiry ) {
        this._Cache[ key ] = (value, DateTime.UtcNow.Add(expiry));
    }

    public bool TryGet( TKey key, out TValue value ) {
        if( this._Cache.TryGetValue(key, out var entry) ) {
            if( entry.expires > DateTime.UtcNow ) {
                value = entry.value;
                return true;
            }
            this._Cache.TryRemove( key, out _ );
        }
        value = default!;
        return false;
    }

    public TValue GetOrAdd( TKey key, TValue value, TimeSpan expiry ) {
        while( true ) {
            if( this.TryGet(key, out var newValue) ) {
                return newValue;
            }
            if( this._Cache.TryAdd(key, (value, DateTime.UtcNow.Add(expiry))) ) {
                return value;
            }
        }
    }

    public bool Remove( TKey key ) {
        return this._Cache.TryRemove( key, out _ );
    }
}
