using Order.Domain.Common;
using Order.Domain.Interfaces.Repositories;
using Order.Domain.Interfaces.Repositories.DataConnector;
using Order.Domain.Interfaces.Services;
using Order.Domain.Services;
using Order.Interfaces;
using Order.Interfaces.Applications;
using Order.Request;
using System.Security.Claims;
using TimeProvider = Order.Domain.Common.TimeProvider;

namespace Order.Extensions
{
    public static class RegisterIoCExtensions
    {
        public static void RegisterIoC(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, MyUnitOfWork>();
            services.AddScoped<ITimeProvider, TimeProvider>();
            services.AddScoped<IGenerators, Generators>();

            services.AddScoped<IClientApplication, IClientApplication>();
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<IClientRepository, IClientRepository>();

            services.AddScoped<IOrderApplication, MyOrderApplication>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderRepository, IOrderRepository>();

            services.AddScoped<IUserApplication, IUserApplication>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, IUserRepository>();

            services.AddScoped<IProductApplication, IProductApplication>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductRepository, IProductRepository>();

            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<ITokenManager, ITokenManager>();
        }
    }

    internal class MyOrderApplication : IOrderApplication
    {
        // Implemente a interface IOrderApplication conforme necessário
        public Task AuthAsync(AuthRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<IOrderApplication.OrderCreateResponse> CreateAsync(CreateOrderRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<IOrderApplication.Order> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<List<IOrderApplication.Order>> ListAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<IOrderApplication.Order>> ListByFilterAsync(string orderId, string clientId, string userId)
        {
            throw new NotImplementedException();
        }

        public Task ListByFilterAsync(ClaimsPrincipal user, object name)
        {
            throw new NotImplementedException();
        }

        public Task ListByFilterAsync(ClaimsPrincipal user, string name)
        {
            throw new NotImplementedException();
        }

        public Task ListByFilterAsync(ClaimsPrincipal claimsPrincipal)
        {
            throw new NotImplementedException();
        }
    }

    internal class MyUnitOfWork : IUnitOfWork
    {
        // Implemente a interface IUnitOfWork conforme necessário
        public IClientRepository ClientRepository => throw new NotImplementedException();

        public IOrderRepository OrderRepository => throw new NotImplementedException();

        public IProductRepository ProductRepository => throw new NotImplementedException();

        public IUserRepository UserRepository => throw new NotImplementedException();

        public IDbConnector dbConnector => throw new NotImplementedException();

        public void BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public void CommitTransaction()
        {
            throw new NotImplementedException();
        }

        public void RollbackTransaction()
        {
            throw new NotImplementedException();
        }
    }

    // Implemente as outras classes conforme necessário
}
