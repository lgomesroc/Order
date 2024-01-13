namespace Order.Application.DataContract.Response.Order
{
    public sealed class OrderResponse
    {
        public string Id { get; set; }
        public string ClientId { get; set; }
        public string UserId { get; set; }
        public List<OrderItemResponse> Items { get; set; }
    }
}
