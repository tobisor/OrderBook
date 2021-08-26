using OrderBook.BL.Models;

namespace OrderBook.BL
{
    public interface IOrdersExecutor
    {
        bool ExecuteOrders(OrderBookEntry buyOrder, OrderBookEntry sellOrder);
    }
}
