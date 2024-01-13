using Order.Domain.Interfaces.Repositories.DataConnector;
using System.Data;

namespace Order.Infra.DataConnector
{
    public class SqlConnector : IDbConnector
    {
        public SqlConnector(string connectionString)
        {
            SqlClientFactory factory = new SqlClientFactory();
            dbConnection = factory.CreateConnection();
            dbConnection.ConnectionString = connectionString;
        }


        public IDbConnection dbConnection { get; }

        public IDbConnection DbConnection => throw new NotImplementedException();

        public IDbTransaction dbTransaction { get; set; }
        public object SqlClientFactory { get; private set; }

        object IDbConnector.dbConnection => throw new NotImplementedException();

        public IDbTransaction BeginTransaction(IsolationLevel isolation)
        {
            if (dbTransaction != null)
            {
                return dbTransaction;
            }

            if (dbConnection.State == ConnectionState.Closed)
            {
                dbConnection.Open();
            }

            return (dbTransaction = dbConnection.BeginTransaction(isolation));
        }

        public void Dispose()
        {
            dbConnection?.Dispose();
            dbTransaction?.Dispose();
        }
    }
}
