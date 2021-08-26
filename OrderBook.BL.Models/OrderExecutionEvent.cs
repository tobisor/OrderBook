
namespace OrderBook.BL.Models
{
    public class OrderExecutionEvent
    {
        public string Exchange { get; set; }

        public string Symbol { get; set; }

        public int Quantity { get; set; }

        public OrderStatus NewStatus { get; set; }
    }
}
