
namespace OrderBook.WebapiService
{
    public class OrderExecutionResult
    {
        public string ExchangeName { get; set; }

        public string Symbol { get; set; }

        public double Quantity { get; set; }

        public string NewStatus { get; set; }
    }
}
