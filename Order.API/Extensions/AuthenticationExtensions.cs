using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Order.Application.Models;

namespace Order.Extensions
{
    public static class AuthenticationExtensions
    {
        private const int MinimumSecretLength = 32;

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = configuration.GetSection(AuthSettings.SectionName).Get<AuthSettings>() ?? new AuthSettings();

            if (string.IsNullOrWhiteSpace(settings.Secret) || settings.Secret.Length < MinimumSecretLength)
                throw new InvalidOperationException(
                    $"AuthSettings:Secret must be configured with at least {MinimumSecretLength} characters. " +
                    "Use user-secrets or the AuthSettings__Secret environment variable.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Secret));

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = settings.Issuer,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        ValidateLifetime = true,
                        RequireExpirationTime = true,
                        ClockSkew = TimeSpan.FromMinutes(1)
                    };
                });

            services.AddAuthorization();

            return services;
        }
    }
}
