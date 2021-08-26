
using System;

namespace OrderBook.BL.Models
{
    public class OrderBookEntry
    {
        public OrderBookEntry(string id = null)
        {
            Id = !string.IsNullOrEmpty(id)
                ? id
                : Guid.NewGuid().ToString();
        }

        public string Exchange { get; set; }

        public double Quantity { get; set; }

        public OrderSide Side { get; set; }

        public string Symbol { get; set; }

        public double Price { get; set; }

        public OrderType OrderType { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public long TimeStamp { get; set; }

        public (string ExchangeName, string Symbol) OrderKey => (Exchange, Symbol);

        public (double Price, long TimeStamp, string orderId) SortingKey => (Price, TimeStamp, Id);

        public string Id { get; set; }
    }
}
