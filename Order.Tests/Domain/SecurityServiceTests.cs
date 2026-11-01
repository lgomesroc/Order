using Order.Domain.Services;

namespace Order.Tests.Domain;

public class SecurityServiceTests
{
    private readonly SecurityService _securityService = new();

    [Theory]
    [InlineData(null, null)]
    [InlineData(null, "password")]
    [InlineData("password", null)]
    [InlineData("", "")]
    public async Task ComparePassword_ReturnsFalse_WhenEitherPasswordIsNullOrEmpty(string password, string confirmation)
    {
        var result = await _securityService.ComparePassword(password, confirmation);

        Assert.False(result.Data);
    }

    [Fact]
    public async Task ComparePassword_ReturnsTrue_WhenPasswordsMatch()
    {
        var result = await _securityService.ComparePassword("StrongPass1!", "StrongPass1!");

        Assert.True(result.Data);
    }

    [Fact]
    public async Task ComparePassword_ReturnsFalse_WhenPasswordsDiffer()
    {
        var result = await _securityService.ComparePassword("StrongPass1!", "DifferentPass1!");

        Assert.False(result.Data);
    }
}
