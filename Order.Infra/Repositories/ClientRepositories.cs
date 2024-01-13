using Dapper;
using Order.Domain.Interfaces.Repositories;
using Order.Domain.Models;
using System.Data;
using SqlConnection = System.Data.SqlClient.SqlConnection;


namespace Order.Infra.Repositories
{
    // Ajuste a interface IDbConnector para incluir a propriedade dbConnection
    public interface IDbConnector : IDisposable
    {
        IDbConnection DbConnection { get; }
        IDbTransaction DbTransaction { get; set; }

        void BeginTransaction(IsolationLevel readUncommitted);
    }

    // Ajuste a classe SqlConnector para implementar a interface de maneira adequada
    public class SqlConnector : IDbConnector
    {
        public SqlConnector(string connectionString)
        {
            DbConnection = new SqlConnection(connectionString);
        }

        public IDbConnection DbConnection { get; }

        public IDbTransaction DbTransaction { get; set; }

        IDbConnection IDbConnector.DbConnection => DbConnection;

        public void BeginTransaction(IsolationLevel readUncommitted)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        // Restante da implementação...
    }

    // Ajuste a classe ClientRepository para utilizar a propriedade DbConnection
    public class ClientRepository : IClientRepository
    {
        private readonly IDbConnector _dbConnector;

        public ClientRepository(IDbConnector dbConnector)
        {
            _dbConnector = dbConnector ?? throw new ArgumentNullException(nameof(dbConnector));
        }

        const string baseSql = @"SELECT [Id]
                                      ,[Name]
                                      ,[Email]
                                      ,[PhoneNumber]
                                      ,[Address]
                                      ,[CreatedAt]
                                  FROM [dbo].[Client]
                                  WHERE 1 = 1 ";

        public async Task CreateAsync(ClientModel client)
        {
            string sql = @"INSERT INTO [dbo].[Client]
                                   ([Id]
                                   ,[Name]
                                   ,[Email]
                                   ,[PhoneNumber]
                                   ,[Address]
                                   ,[CreatedAt])
                             VALUES
                                   (@Id
                                   ,@Name
                                   ,@Email
                                   ,@PhoneNumber
                                   ,@Address
                                   ,@CreatedAt)";

            await _dbConnector.DbConnection.ExecuteAsync(sql, new
            {
                Id = client.Id,
                Name = client.Name,
                Email = client.Email,
                PhoneNumber = client.PhoneNumber,
                Address = client.Address,
                CreatedAt = client.CreatedAt
            }, _dbConnector.DbTransaction);
        }

        public async Task UpdateAsync(ClientModel client)
        {
            string sql = @"UPDATE [dbo].[Client]
                               SET [Name] = @Name
                                  ,[Email] = @Email
                                  ,[PhoneNumber] = @PhoneNumber
                                  ,[Address] = @Address
                             WHERE Id = @Id";

            await _dbConnector.DbConnection.ExecuteAsync(sql, new
            {
                Id = client.Id,
                Name = client.Name,
                Email = client.Email,
                PhoneNumber = client.PhoneNumber,
                Address = client.Address,
            }, _dbConnector.DbTransaction);
        }

        public async Task DeleteAsync(string clientId)
        {
            string sql = $"DELETE FROM [dbo].[Client] WHERE Id = @Id";

            await _dbConnector.DbConnection.ExecuteAsync(sql, new { Id = clientId }, _dbConnector.DbTransaction);
        }

        public async Task<bool> ExistsByIdAsync(string clientId)
        {
            string sql = $"SELECT 1 FROM Client WHERE Id = @Id ";

            var clients = await _dbConnector.DbConnection.QueryAsync<bool>(sql, new { Id = clientId }, _dbConnector.DbTransaction);

            return clients.FirstOrDefault();
        }

        public async Task<ClientModel> GetByIdAsync(string clientId)
        {
            string sql = $"{baseSql} AND Id = @Id";

            var clients = await _dbConnector.DbConnection.QueryAsync<ClientModel>(sql, new { Id = clientId }, _dbConnector.DbTransaction);

            return clients.FirstOrDefault();
        }

        public async Task<List<ClientModel>> ListByFilterAsync(string clientId = null, string name = null)
        {
            string sql = $"{baseSql} ";

            if (!string.IsNullOrWhiteSpace(clientId))
                sql += "AND Id = @Id";

            if (!string.IsNullOrWhiteSpace(name))
                sql += "AND Name like @Name";

            var clients = await _dbConnector.DbConnection.QueryAsync<ClientModel>(sql, new { Id = clientId, Name = "%" + name + "%" }, _dbConnector.DbTransaction);

            return clients?.ToList() ?? new List<ClientModel>();
        }

        public Task DeleteAsync(ClientModel client)
        {
            throw new NotImplementedException();
        }
    }
}
