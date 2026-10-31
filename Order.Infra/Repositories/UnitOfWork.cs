using System.Data;
using Order.Domain.Interfaces.Repositories;
using Order.Domain.Interfaces.Repositories.DataConnector;

namespace Order.Infra.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private IClientRepository _clientRepository;
        private IProductRepository _productRepository;
        private IOrderRepository _orderRepository;
        private IUserRepository _userRepository;

        public UnitOfWork(IDbConnector dbConnector)
        {
            this.dbConnector = dbConnector;
        }

        public IDbConnector dbConnector { get; }

        public IClientRepository ClientRepository => _clientRepository ??= new ClientRepository(dbConnector);
        public IOrderRepository OrderRepository => _orderRepository ??= new OrderRepository(dbConnector);
        public IProductRepository ProductRepository => _productRepository ??= new ProductRepository(dbConnector);
        public IUserRepository UserRepository => _userRepository ??= new UserRepository(dbConnector);

        public void BeginTransaction() => dbConnector.BeginTransaction(IsolationLevel.ReadCommitted);

        public void CommitTransaction() => dbConnector.Commit();

        public void RollbackTransaction() => dbConnector.Rollback();
    }
}
