using Microsoft.Extensions.Logging;
using Order.Application.DataContract.Request.User;
using Order.Application.DataContract.Response.User;
using Order.Application.Interfaces;
using Order.Application.Interfaces.Security;
using Order.Application.Mapper;
using Order.Domain.Interfaces.Services;
using Order.Domain.Validations.Base;

namespace Order.Application.Applications;

public class UserApplication(
    IUserService userService,
    ISecurityService securityService,
    ITokenManager tokenManager,
    ILogger<UserApplication> logger) : IUserApplication
{
    public async Task<Response<AuthResponse>> AuthAsync(AuthRequest auth)
    {
        var user = await userService.GetByLoginAsync(auth.Login);
        if (user.Report.Any()) return Response.Unprocessable<AuthResponse>(user.Report);

        var authenticated = await userService.AutheticationAsync(auth.Password, user.Data);
        if (!authenticated.Data)
            return Response.Unprocessable<AuthResponse>(new List<Report> { Report.Create("Invalid password or login") });

        var token = await tokenManager.GenerateTokenAsync(user.Data);
        return new Response<AuthResponse>(token);
    }

    public async Task<Response> CreateAsync(CreateUserRequest request)
    {
        try
        {
            var passwordsMatch = await securityService.ComparePassword(request.Password, request.ConfirmPassword);
            if (!passwordsMatch.Data)
                return Response.Unprocessable(Report.Create("Passwords do not match"));

            var encryptedPassword = await securityService.EncryptPassword(request.Password);
            if (encryptedPassword.Report.Any())
                return Response.Unprocessable(encryptedPassword.Report);

            request.Password = encryptedPassword.Data;
            return await userService.CreateAsync(ApplicationMapper.ToModel(request));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while creating a user.");
            return Response.Unprocessable(Report.Create("The user could not be created."));
        }
    }

    public async Task<Response<List<UserResponse>>> ListByFilterAsync(string userId = null, string name = null)
    {
        try
        {
            var result = await userService.ListByFilterAsync(userId, name);
            return result.Report.Any()
                ? Response.Unprocessable<List<UserResponse>>(result.Report)
                : Response.OK(result.Data.Select(ApplicationMapper.ToResponse).ToList());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while listing users.");
            return Response.Unprocessable<List<UserResponse>>(
                new List<Report> { Report.Create("Users could not be retrieved.") });
        }
    }
}
