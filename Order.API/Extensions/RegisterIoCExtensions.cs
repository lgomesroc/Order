using Order.Application.Applications;
using Order.Application.Interfaces;
using Order.Application.Interfaces.Security;
using Order.Application.Mapper;
using Order.Application.Models;
using Order.Application.Security;
using Order.Domain.Common;
using Order.Domain.Interfaces.Repositories;
using Order.Domain.Interfaces.Repositories.DataConnector;
using Order.Domain.Interfaces.Services;
using Order.Domain.Services;
using Order.Infra.DataConnector;
using Order.Infra.Repositories;

namespace Order.Extensions
{
    public static class RegisterIoCExtensions
    {
        public static IServiceCollection RegisterIoC(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Default");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException(
                    "Connection string 'Default' is not configured. " +
                    "Set it with user-secrets or the ConnectionStrings__Default environment variable.");

            services.AddAutoMapper(typeof(Core));
            services.Configure<AuthSettings>(configuration.GetSection(AuthSettings.SectionName));

            // One connection (and transaction) per request, shared by every repository.
            services.AddScoped<IDbConnector>(_ => new SqlConnector(connectionString));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddSingleton<Order.Domain.Common.ITimeProvider, Order.Domain.Common.TimeProvider>();
            services.AddSingleton<IGenerators, Generators>();

            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISecurityService, SecurityService>();

            services.AddScoped<IClientApplication, ClientApplication>();
            services.AddScoped<IProductApplication, ProductApplication>();
            services.AddScoped<IOrderApplication, OrderApplication>();
            services.AddScoped<IUserApplication, UserApplication>();
            services.AddScoped<ITokenManager, TokenManager>();

            return services;
        }
    }
}
