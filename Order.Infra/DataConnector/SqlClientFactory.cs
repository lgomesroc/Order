using System.Data;
using System.Data.SqlClient;
namespace Order.Infra.DataConnector { public class SqlClientFactory { public IDbConnection CreateConnection() => new SqlConnection(); } }