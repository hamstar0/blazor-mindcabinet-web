using System.Collections.Concurrent;
using System.Data;
using MindCabinet.Shared.Utility;


namespace MindCabinet.Shared.Utility;



public class SimpleCache<TKey, TValue>( bool refreshExpiryOnGet ) 
                where TKey : notnull {
    private readonly ConcurrentDictionary<TKey, (TValue value, TimeSpan expires)> _Cache = new();

    private readonly ConcurrentDictionary<TKey, DateTime> _LastUpdated = new();

    private bool RefreshOnGet = refreshExpiryOnGet;



    public void Set( TKey key, TValue value, TimeSpan expiry ) {
        this._Cache[ key ] = (value, expiry);
        this._LastUpdated[ key ] = DateTime.UtcNow;
    }


    public bool TryGet( TKey key, out TValue value ) {
        if( this._Cache.TryGetValue(key, out var entry) ) {
            if( (this._LastUpdated[key] + entry.expires) > DateTime.UtcNow ) {
                if( this.RefreshOnGet ) {
                    this._LastUpdated[key] = DateTime.UtcNow;
                }

                value = entry.value;
                return true;
            }

            this._Cache.TryRemove( key, out _ );
            this._LastUpdated.TryRemove( key, out _ );
        }
        value = default!;

        return false;
    }

    public TValue GetOrAdd( TKey key, TValue value, TimeSpan expiry ) {
        var entry = this._Cache.GetOrAdd( key, (value, expiry) );
        this._LastUpdated[key] = DateTime.UtcNow;
        return entry.value;
    }

    public TValue GetOrAdd( TKey key, Func<TValue> valueFactory, TimeSpan expiry ) {
        var entry = this._Cache.GetOrAdd( key, _ => (valueFactory(), expiry) );
        this._LastUpdated[key] = DateTime.UtcNow;
        return entry.value;
    }


    public IEnumerable<TValue> GetMany( IEnumerable<TKey> keys ) {
        foreach( TKey key in keys ) {
            if( this.TryGet(key, out var value) ) {
                yield return value;
            }
        }
    }


    public bool Remove( TKey key ) {
        bool removed = this._Cache.TryRemove( key, out _ );
        this._LastUpdated.TryRemove( key, out _ );
        return removed;
    }
}
