using OrderBook.BL.Models;
using System.Collections.Generic;

namespace OrderBook.WebapiService
{
    public class OrderBookModel
    {
        public OrderBookModel()
        {
        }

        public OrderBookModel(string exchangeName, string symbol, IEnumerable<OrderBookEntry> bids, IEnumerable<OrderBookEntry> asks)
        {
            ExchangeName = exchangeName;
            Symbol = symbol;
            Bids = bids;
            Asks = asks;
        }

        public string ExchangeName { get; set; }

        public string Symbol { get; set; }

        public IEnumerable<OrderBookEntry> Bids { get; set; }

        public IEnumerable<OrderBookEntry> Asks { get; set; }
    }
}
