using OrderBook.BL.Models;
using System.Collections.Generic;

namespace OrderBook.BL
{
    public interface IOrderBookEngine
    {
        void Start();

        void PushOrders(IEnumerable<OrderBookEntry> orders);

        IList<OrderBookEntry> GetBids();

        IList<OrderBookEntry> GetAsks();
    }
}
