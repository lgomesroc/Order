using Order.Request;
using System.Security.Claims;

namespace Order.Interfaces.Applications
{
    public interface IOrderApplication
    {
        Task AuthAsync(AuthRequest request);
        Task<OrderCreateResponse> CreateAsync(CreateOrderRequest request);
        Task<List<Order>> ListAsync();
        Task<List<Order>> ListByFilterAsync(string orderId, string clientId, string userId);
        Task ListByFilterAsync(ClaimsPrincipal user, object name);
        Task ListByFilterAsync(ClaimsPrincipal user, string name);
        Task ListByFilterAsync(ClaimsPrincipal claimsPrincipal);
        Task<Order> GetByIdAsync(string id);

        public class OrderCreateResponse
        {
            public List<ReportItem> Report { get; set; }
            public string OrderId { get; set; }
            public bool Success { get; set; }
        }
        public class Order
        {
            public string OrderId { get; set; }
            public string ClientId { get; set; }
            public string UserId { get; set; }
            public DateTime OrderDate { get; set; }
            public decimal TotalAmount { get; set; }
            public List<OrderItem> OrderItems { get; set; }
            public List<OrderResponse> Report { get; set; }

            public OrderStatus Status { get; set; }
            public void AddItem(OrderItem item)
            {
                if (OrderItems == null)
                    OrderItems = new List<OrderItem>();

                OrderItems.Add(item);
            }
            public decimal CalculateTotalAmount()
            {
                decimal total = 0;

                if (OrderItems != null)
                {
                    foreach (var item in OrderItems)
                    {
                        total += item.Price * item.Quantity;
                    }
                }

                return total;
            }
        }
        public class OrderItem
        {
            public string ProductId { get; set; }
            public string ProductName { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
        }
        public enum OrderStatus
        {
            Pending,
            Processing,
            Shipped,
            Completed,
            Canceled
        }
        public class OrderResponse
        {
            public List<Order> Orders { get; set; }
            public List<ReportItem> Report { get; set; }
        }
    }
}