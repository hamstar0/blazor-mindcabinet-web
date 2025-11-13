using System.Collections.Frozen;

namespace MindCabinet.Shared.Utility;

public static class DictionaryHelpers {
    public static IReadOnlyDictionary<TKey, TValue> CreateFrozen<TKey, TValue>(
            IList<(TKey, TValue)> source ) where TKey : notnull {
        return source.ToFrozenDictionary( kv => kv.Item1, kv => kv.Item2 );
    }


    public static TValue? GetValueOrDefault<TKey, TValue>(
            this IDictionary<TKey, TValue> dict,
            TKey key ) {
        if( dict.TryGetValue(key, out TValue? value) ) {
            return value;
        } else {
            return default( TValue );
        }
    }
}
