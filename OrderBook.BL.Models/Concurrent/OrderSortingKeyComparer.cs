using System.Collections.Generic;

namespace OrderBook.BL.Models.Concurrent
{
    public class OrderSortingKeyComparer : IComparer<(double price, long timestamp, string orderId)>
    {
        public int Compare((double price, long timestamp, string orderId) x, (double price, long timestamp, string orderId) y)
        {
            switch (x.price.CompareTo(y.price))
            {
                case -1:
                case 1:
                    return x.price.CompareTo(y.price);
                case 0:
                    if (x.timestamp.CompareTo(y.timestamp) == 0)
                    {
                        return x.orderId.CompareTo(y.orderId);
                    }
                    return x.timestamp.CompareTo(y.timestamp);
                default:
                    return default;
            }
        }
    }

    public class ReverseOrderSortingKeyComparer : IComparer<(double price, long timestamp, string orderId)>
    {
        public int Compare((double price, long timestamp, string orderId) x, (double price, long timestamp, string orderId) y)
        {
            switch (y.price.CompareTo(x.price))
            {
                case -1:
                case 1:
                    return y.price.CompareTo(x.price);
                case 0:
                    if (y.timestamp.CompareTo(x.timestamp) == 0)
                    {
                        return y.orderId.CompareTo(x.orderId);
                    }
                    return y.timestamp.CompareTo(x.timestamp);
                default:
                    return default;
            }
        }
    }
}
