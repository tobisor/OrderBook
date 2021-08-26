
using System.Collections.Generic;

namespace OrderBook.BL.Models.Concurrent
{
    public class OrdersSortedSet : IOrdersSortedSet
    {
        private IConcurrentSortedSet<(double price, long timestamp, string orderId), OrderBookEntry> _ordersSet;

        public OrdersSortedSet(bool isAscOrder = true)
        {
            if (isAscOrder)
            {
                _ordersSet = new ConcurrentSortedSet<(double price, long timestamp, string orderId), OrderBookEntry>(new OrderSortingKeyComparer());
            }
            else
            {
                _ordersSet = new ConcurrentSortedSet<(double price, long timestamp, string orderId), OrderBookEntry>(new ReverseOrderSortingKeyComparer());
            }
        }

        public void Add(OrderBookEntry order)
        {
            _ordersSet.Add(order.SortingKey, order);
        }

        public OrderBookEntry RemoveAndGetFirstOrDefault() => _ordersSet.RemoveAndGetFirstOrDefault();

        public OrderBookEntry RemoveAndGetLastOrDefault() => _ordersSet.RemoveAndGetLastOrDefault();

        public List<OrderBookEntry> ToList() => _ordersSet.ToList();

        public int Count => _ordersSet.Count;
    }
}
