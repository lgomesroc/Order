namespace Order.Application.DataContract.Request.Order
{
    public sealed class UpdateOrderRequest
    {
        public string Id { get; set; }
        public string ClientId { get; set; }
        public string UserId { get; set; }
        public List<CreateOrderItemRequest> Items { get; set; }
    }
}
