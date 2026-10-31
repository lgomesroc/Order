using System.Data;

namespace Order.Domain.Interfaces.Repositories.DataConnector
{
    /// <summary>
    /// Abstraction over the database connection and the current transaction.
    /// One instance is shared by all repositories inside the same request scope.
    /// </summary>
    public interface IDbConnector : IDisposable
    {
        IDbConnection DbConnection { get; }
        IDbTransaction DbTransaction { get; }

        IDbTransaction BeginTransaction(IsolationLevel isolation);
        void Commit();
        void Rollback();
    }
}
