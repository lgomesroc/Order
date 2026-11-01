using Order.Domain.Models;
using Order.Domain.Validations;

namespace Order.Tests.Domain;

public class ValidationTests
{
    [Fact]
    public void ProductValidation_RejectsEmptyDescriptionAndZeroPrice()
    {
        var result = new ProductValidation().Validate(new ProductModel
        {
            Description = "",
            SellValue = 0,
            Stock = 0
        });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(ProductModel.Description));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(ProductModel.SellValue));
    }

    [Fact]
    public void ProductValidation_AcceptsValidProduct()
    {
        var result = new ProductValidation().Validate(new ProductModel
        {
            Description = "Keyboard",
            SellValue = 129.90m,
            Stock = 10
        });

        Assert.True(result.IsValid);
    }

    [Fact]
    public void ProductValidation_RejectsNegativeStock()
    {
        var result = new ProductValidation().Validate(new ProductModel
        {
            Description = "Keyboard",
            SellValue = 10m,
            Stock = -1
        });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(ProductModel.Stock));
    }

    [Fact]
    public void ClientValidation_RejectsInvalidEmail()
    {
        var result = new ClientValidation().Validate(new ClientModel
        {
            Name = "Maria Silva",
            Email = "not-an-email",
            PhoneNumber = "21999990000"
        });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(ClientModel.Email));
    }

    [Fact]
    public void UserValidation_RejectsShortPasswordHash()
    {
        var result = new UserValidation().Validate(new UserModel
        {
            Name = "Maria Silva",
            Login = "maria",
            PasswordHash = "123"
        });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UserModel.PasswordHash));
    }

    [Fact]
    public void OrderValidation_RequiresAtLeastOneItem()
    {
        var result = new OrderValidation().Validate(new OrderModel
        {
            Client = new ClientModel(),
            User = new UserModel(),
            Items = new List<OrderItemModel>()
        });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.ErrorMessage == "An order must contain at least one item.");
    }

    [Fact]
    public void OrderValidation_RejectsNonPositiveItemQuantity()
    {
        var result = new OrderValidation().Validate(new OrderModel
        {
            Client = new ClientModel(),
            User = new UserModel(),
            Items = new List<OrderItemModel>
            {
                new() { Product = new ProductModel(), Quantity = 0 }
            }
        });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName.Contains("Quantity"));
    }
}
