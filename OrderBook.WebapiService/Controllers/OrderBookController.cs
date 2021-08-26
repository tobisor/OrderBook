using Microsoft.AspNetCore.Mvc;
using OrderBook.BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderBook.WebapiService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OrderBookController : ControllerBase
    {
        private OrderEngineWorker _orderEngineWorker;

        public OrderBookController(OrderEngineWorker orderEngineWorker)
        {
            _orderEngineWorker = orderEngineWorker;
        }

        [HttpGet]
        public ActionResult<List<OrderBookModel>> GetAll()
        {
            var books = _orderEngineWorker.GetAllBooks();
            if (books == null)
                return new NotFoundResult();
            else
                return books.ToList();
        }

        [HttpGet("fetch")]
        public ActionResult<OrderBookModel> Get(string exchange, string symbol)
        {
            var book = _orderEngineWorker.GetBook(exchange, symbol);
            if (book == null)
                return new NotFoundResult();
            else
                return book;
        }

        [HttpPost("update")]
        public ActionResult<OrderExecutionResult> Update(OrderBookEntry orderBookEntry)
        {
            orderBookEntry.Id = string.IsNullOrEmpty(orderBookEntry.Id) ? Guid.NewGuid().ToString() : orderBookEntry.Id;
            var quantity = orderBookEntry.Quantity;
            var exchange = orderBookEntry.Exchange;
            var symbol = orderBookEntry.Symbol;
            var side = orderBookEntry.Side;
            var orderId = orderBookEntry.Id;
            _orderEngineWorker.PushOrders(new[] { orderBookEntry });
            var book = _orderEngineWorker.GetBook(exchange, symbol);
            var updatedOrder = GetOrderFromBook(book, side, orderId);
            return HandleUpdateResult(quantity, exchange, symbol, updatedOrder);
        }

        private static ActionResult<OrderExecutionResult> HandleUpdateResult(double quantity, string exchange, string symbol, OrderBookEntry updatedOrder)
        {
            if (updatedOrder == null)
            {
                return new OrderExecutionResult
                {
                    NewStatus = OrderStatus.Execute.ToString(),
                    Quantity = quantity,
                    Symbol = symbol,
                    ExchangeName = exchange
                };
            }
            else
            {
                return new OrderExecutionResult
                {
                    NewStatus = OrderStatus.New.ToString(),
                    Quantity = quantity - updatedOrder.Quantity,
                    Symbol = symbol,
                    ExchangeName = exchange
                };
            }
        }

        private OrderBookEntry GetOrderFromBook(OrderBookModel book, OrderSide side, string orderId)
        {
            if (side.Equals(OrderSide.Buy))
            {
                return book.Bids.SingleOrDefault(b => b.Id.Equals(orderId));
            }
            else
            {
                return book.Asks.SingleOrDefault(b => b.Id.Equals(orderId));
            }
        }
    }
}
