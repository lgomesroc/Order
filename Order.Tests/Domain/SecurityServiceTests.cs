using Order.Domain.Services;

namespace Order.Tests.Domain;

public class SecurityServiceTests
{
    [Theory]
    [InlineData(null, "x")]
    [InlineData("x", null)]
    [InlineData(null, null)]
    public async Task ComparePassword_NullInput_ReturnsFalseInsteadOfThrowing(string password, string confirm)
    {
        var result = await new SecurityService().ComparePassword(password, confirm);
        Assert.False(result.Data);
    }

    [Fact]
    public async Task ComparePassword_EqualValues_ReturnsTrue()
    {
        var result = await new SecurityService().ComparePassword("Demo@1234", " Demo@1234 ");
        Assert.True(result.Data);
    }
}
