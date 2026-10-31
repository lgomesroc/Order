using System.Data;
using Microsoft.Data.SqlClient;
using Order.Domain.Interfaces.Repositories.DataConnector;

namespace Order.Infra.DataConnector
{
    /// <summary>
    /// SQL Server implementation of <see cref="IDbConnector"/>.
    /// Registered as scoped: one connection (and at most one transaction) per HTTP request.
    /// </summary>
    public class SqlConnector : IDbConnector
    {
        private readonly SqlConnection _connection;

        public SqlConnector(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string is required.", nameof(connectionString));

            _connection = new SqlConnection(connectionString);
        }

        public IDbConnection DbConnection => _connection;

        public IDbTransaction DbTransaction { get; private set; }

        public IDbTransaction BeginTransaction(IsolationLevel isolation)
        {
            if (DbTransaction != null)
                return DbTransaction;

            if (_connection.State == ConnectionState.Closed)
                _connection.Open();

            return DbTransaction = _connection.BeginTransaction(isolation);
        }

        public void Commit()
        {
            if (DbTransaction == null)
                return;

            DbTransaction.Commit();
            DbTransaction.Dispose();
            DbTransaction = null;
        }

        public void Rollback()
        {
            if (DbTransaction == null)
                return;

            DbTransaction.Rollback();
            DbTransaction.Dispose();
            DbTransaction = null;
        }

        public void Dispose()
        {
            DbTransaction?.Dispose();
            DbTransaction = null;
            _connection.Dispose();
        }
    }
}
