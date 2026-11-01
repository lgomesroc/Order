using Order.Application.DataContract.Request.Client;
using Order.Application.DataContract.Request.Order;
using Order.Application.DataContract.Request.Product;
using Order.Application.DataContract.Request.User;
using Order.Application.DataContract.Response.Client;
using Order.Application.DataContract.Response.Order;
using Order.Application.DataContract.Response.Product;
using Order.Application.DataContract.Response.User;
using Order.Domain.Models;

namespace Order.Application.Mapper;

public static class ApplicationMapper
{
    public static ClientModel ToModel(CreateClientRequest source) => new()
    {
        Name = source.Name,
        Email = source.Email,
        PhoneNumber = source.PhoneNumber,
        Address = source.Address
    };

    public static ClientModel ToModel(UpdateClientRequest source) => new()
    {
        Id = source.Id,
        Name = source.Name,
        Email = source.Email,
        PhoneNumber = source.PhoneNumber,
        Address = source.Address
    };

    public static clientResponse ToResponse(ClientModel source) => new()
    {
        Id = source.Id,
        Name = source.Name,
        Email = source.Email,
        PhoneNumber = source.PhoneNumber,
        Address = source.Address
    };

    public static ProductModel ToModel(CreateProductRequest source) => new()
    {
        Description = source.Description,
        SellValue = source.SellValue,
        Stock = source.Stock
    };

    public static ProductModel ToModel(UpdateProductRequest source) => new()
    {
        Id = source.Id,
        Description = source.Description,
        SellValue = source.SellValue,
        Stock = source.Stock
    };

    public static ProductResponse ToResponse(ProductModel source) => new()
    {
        Id = source.Id,
        Description = source.Description,
        SellValue = source.SellValue,
        Stock = source.Stock
    };

    public static UserModel ToModel(CreateUserRequest source) => new()
    {
        Name = source.Name,
        Login = source.Login,
        PasswordHash = source.Password
    };

    public static UserResponse ToResponse(UserModel source) => new()
    {
        Id = source.Id,
        Name = source.Name,
        Login = source.Login
    };

    public static OrderModel ToModel(CreateOrderRequest source) => new()
    {
        Client = new ClientModel { Id = source.ClientId },
        User = new UserModel { Id = source.UserId },
        Items = source.Items?.Select(ToModel).ToList() ?? new List<OrderItemModel>()
    };

    public static OrderModel ToModel(UpdateOrderRequest source) => new()
    {
        Id = source.Id,
        Client = new ClientModel { Id = source.ClientId },
        User = new UserModel { Id = source.UserId },
        Items = source.Items?.Select(ToModel).ToList() ?? new List<OrderItemModel>()
    };

    public static OrderItemModel ToModel(CreateOrderItemRequest source) => new()
    {
        Product = new ProductModel { Id = source.ProductId },
        SellValue = source.SellValue,
        Quantity = source.Quantity
    };

    public static OrderResponse ToResponse(OrderModel source) => new()
    {
        Id = source.Id,
        ClientId = source.Client?.Id,
        UserId = source.User?.Id,
        Items = source.Items?.Select(ToResponse).ToList() ?? new List<OrderItemResponse>()
    };

    public static OrderItemResponse ToResponse(OrderItemModel source) => new()
    {
        ProductId = source.Product?.Id,
        SellValue = source.SellValue,
        Quantity = source.Quantity,
        TotalAmount = source.TotalAmount
    };
}
