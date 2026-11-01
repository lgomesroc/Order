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
    ITokenManager tokenManager) : IUserApplication
{
    public async Task<Response<AuthResponse>> AuthAsync(AuthRequest auth)
    {
        var invalidCredentials = Response.Unprocessable<AuthResponse>(
            Report.Create("Login or password is invalid."));

        if (auth is null || string.IsNullOrWhiteSpace(auth.Login) || string.IsNullOrWhiteSpace(auth.Password))
            return invalidCredentials;

        try
        {
            var user = await userService.GetByLoginAsync(auth.Login.Trim());
            if (user.Report.Any() || user.Data is null)
                return invalidCredentials;

            var authenticated = await userService.AutheticationAsync(auth.Password, user.Data);
            if (authenticated.Report.Any() || !authenticated.Data)
                return invalidCredentials;

            var token = await tokenManager.GenerateTokenAsync(user.Data);
            return new Response<AuthResponse>(token);
        }
        catch
        {
            return invalidCredentials;
        }
    }

    public async Task<Response> CreateAsync(CreateUserRequest request)
    {
        try
        {
            if (request is null || string.IsNullOrWhiteSpace(request.Login))
                return Response.Unprocessable(Report.Create("Login is required."));

            request.Login = request.Login.Trim();
            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
                return Response.Unprocessable(Report.Create("Password must contain at least 8 characters."));

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
            return Response.Unprocessable(Report.Create("Unable to create account."));
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
            return Response.Unprocessable<List<UserResponse>>(new List<Report> { Report.Create(ex.Message) });
        }
    }
}
