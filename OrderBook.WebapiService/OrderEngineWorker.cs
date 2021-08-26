using OrderBook.BL;
using OrderBook.BL.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OrderBook.WebapiService
{
    public class OrderEngineWorker
    {
        private readonly ConcurrentDictionary<(string exchangeName, string symbol), OrderBookEngine> _orderBooks;

        public OrderEngineWorker()
        {
            _orderBooks = new ConcurrentDictionary<(string exchangeName, string symbol), OrderBookEngine>();
        }

        public void PushOrders(IEnumerable<OrderBookEntry> orders)
        {
            var groups = orders.GroupBy(order => order.OrderKey);
            Parallel.ForEach(groups, ordersGroup =>
            {
                (string ExchangeName, string Symbol) key = ordersGroup.Key;
                OrderBookEngine engine = null;
                while (engine == null)
                {
                    if (!_orderBooks.TryGetValue(key, out engine))
                    {
                        var newBook = new OrderBookEngine(key.ExchangeName, key.Symbol);
                        _orderBooks.TryAdd(key, newBook);
                        new Task(newBook.Start).Start();
                    }
                }

                var groupOrders = ordersGroup.Select(o => o);
                engine.PushOrders(groupOrders);
            });
        }

        public IEnumerable<OrderBookModel> GetAllBooks()
        {
            return _orderBooks.Select(b => GetBook(b.Key.exchangeName, b.Key.symbol));
        }


        public OrderBookModel GetBook(string exchangeName, string symbol)
        {
            if(_orderBooks.TryGetValue((exchangeName, symbol), out var book))
            {
                return new OrderBookModel()
                {
                    ExchangeName = book.Id.exchangeName,
                    Symbol = book.Id.symbol,
                    Bids = book.GetBids(),
                    Asks = book.GetAsks()
                };
            }

            return null;
        }
    }
}
