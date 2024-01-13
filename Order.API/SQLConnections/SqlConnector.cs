using Order.Domain.Interfaces.Repositories.DataConnector;
using System.Data;

internal class SqlConnector : IDbConnector
{
    public SqlConnector(string connectionString)
    {
    }

    public IDbConnection DbConnection => throw new NotImplementedException();

    public object dbConnection => throw new NotImplementedException();

    public IDbTransaction dbTransaction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public IDbTransaction BeginTransaction(IsolationLevel isolation)
    {
        throw new NotImplementedException();
    }
}