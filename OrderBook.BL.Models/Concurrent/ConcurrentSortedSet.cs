
using System.Collections.Generic;
using System.Linq;

namespace OrderBook.BL.Models.Concurrent
{
    public class ConcurrentSortedSet<TKey, TValue> : IConcurrentSortedSet<TKey, TValue>
    {
        private SortedList<TKey, TValue> _list;
        private object _locker = new object();

        public ConcurrentSortedSet()
        {
            _list = new SortedList<TKey, TValue>();
        }

        public ConcurrentSortedSet(IComparer<TKey> comparer)
        {
            _list = new SortedList<TKey, TValue>(comparer);
        }

        public void Add(TKey key, TValue value)
        {
            lock (_locker)
            {
                _list.Add(key, value);
            }
        }

        public TValue RemoveAndGetFirstOrDefault()
        {
            lock (_locker)
            {
                TValue val = default(TValue);
                if (_list.Any())
                {
                    val = _list.First().Value;
                    _list.RemoveAt(0);
                }

                return val;
            }
        }
        public TValue RemoveAndGetLastOrDefault()
        {
            lock (_locker)
            {
                TValue val = default(TValue);
                if (_list.Any())
                {
                    var count = _list.Count;
                    val = _list.ElementAt(count).Value;
                    _list.RemoveAt(count);
                }

                return val;
            }
        }

        public int Count => _list.Count;

        public List<TValue> ToList()
        {
            lock (_locker)
            {
                return _list.Values.ToList();
            }
        }
    }
}
