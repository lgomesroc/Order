using System.Data;

namespace Order.Domain.Interfaces.Repositories.DataConnector
{
    public interface IDbConnector
    {
        IDbConnection DbConnection { get; }
        object dbConnection { get; }
        IDbTransaction dbTransaction { get; set; }

        IDbTransaction BeginTransaction(IsolationLevel isolation);
    }
}
