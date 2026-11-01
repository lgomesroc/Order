using Order.Application.DataContract.Request.Order;
using Order.Application.DataContract.Response.Order;
using Order.Application.Mapper;
using Order.Domain.Models;

namespace Order.Tests.Application;

public class ApplicationMapperTests
{
    [Fact]
    public void CreateOrderRequest_MapsRelatedEntityIdsAndItems()
    {
        var result = ApplicationMapper.ToModel(new CreateOrderRequest
        {
            ClientId = "client-1",
            UserId = "user-1",
            Items = new List<CreateOrderItemRequest>
            {
                new() { ProductId = "product-1", Quantity = 3, SellValue = 15m }
            }
        });

        Assert.Equal("client-1", result.Client.Id);
        Assert.Equal("user-1", result.User.Id);
        var item = Assert.Single(result.Items);
        Assert.Equal("product-1", item.Product.Id);
        Assert.Equal(3, item.Quantity);
    }

    [Fact]
    public void OrderModel_MapsToResponseWithoutExposingNestedEntities()
    {
        var result = ApplicationMapper.ToResponse(new OrderModel
        {
            Id = "order-1",
            Client = new ClientModel { Id = "client-1" },
            User = new UserModel { Id = "user-1" },
            Items = new List<OrderItemModel>
            {
                new()
                {
                    Product = new ProductModel { Id = "product-1" },
                    SellValue = 15m,
                    Quantity = 2,
                    TotalAmount = 30m
                }
            }
        });

        Assert.Equal("order-1", result.Id);
        Assert.Equal("client-1", result.ClientId);
        Assert.Equal("user-1", result.UserId);
        var item = Assert.Single(result.Items);
        Assert.Equal("product-1", item.ProductId);
        Assert.Equal(30m, item.TotalAmount);
    }
}
