using System.Collections.Generic;

namespace OrderBook.BL.Models.Concurrent
{
    public interface IConcurrentSortedSet<TKey, TValue>
    {
        int Count { get; }

        void Add(TKey key, TValue value);

        TValue RemoveAndGetFirstOrDefault();

        TValue RemoveAndGetLastOrDefault();

        List<TValue> ToList();
    }
}