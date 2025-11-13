using System.Collections.Frozen;

namespace MindCabinet.Shared.Utility;

public class ListHelpers {
    public static IEnumerable<TValue> PullByIndexes<TValue>( IList<TValue> list, IList<int> orderedIndexes ) {
        var queue = new Queue<TValue>();
        for( int i = orderedIndexes.Count-1; i >= 0; i-- ) {
            queue.Enqueue( list[ orderedIndexes[i] ] );
            list.RemoveAt( orderedIndexes[i] );
        }
        return queue;
    }
}
