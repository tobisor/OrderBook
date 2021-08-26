using System.Collections.Generic;

namespace OrderBook.BL.Models.Concurrent
{
    public interface IOrdersSortedSet
    {
        int Count { get; }

        void Add(OrderBookEntry order);

        OrderBookEntry RemoveAndGetFirstOrDefault();

        OrderBookEntry RemoveAndGetLastOrDefault();

        List<OrderBookEntry> ToList();
    }
}