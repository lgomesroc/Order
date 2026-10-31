using Dapper;
using Order.Domain.Interfaces.Repositories;
using Order.Domain.Interfaces.Repositories.DataConnector;
using Order.Domain.Models;

namespace Order.Infra.Repositories
{
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
                                  ([Id], [Name], [Email], [PhoneNumber], [Address], [CreatedAt])
                           VALUES (@Id, @Name, @Email, @PhoneNumber, @Address, @CreatedAt)";

            await _dbConnector.DbConnection.ExecuteAsync(sql, new
            {
                client.Id,
                client.Name,
                client.Email,
                client.PhoneNumber,
                client.Address,
                client.CreatedAt
            }, _dbConnector.DbTransaction);
        }

        public async Task UpdateAsync(ClientModel client)
        {
            string sql = @"UPDATE [dbo].[Client]
                              SET [Name] = @Name
                                 ,[Email] = @Email
                                 ,[PhoneNumber] = @PhoneNumber
                                 ,[Address] = @Address
                            WHERE [Id] = @Id";

            await _dbConnector.DbConnection.ExecuteAsync(sql, new
            {
                client.Id,
                client.Name,
                client.Email,
                client.PhoneNumber,
                client.Address
            }, _dbConnector.DbTransaction);
        }

        public async Task DeleteAsync(string clientId)
        {
            string sql = "DELETE FROM [dbo].[Client] WHERE [Id] = @Id";

            await _dbConnector.DbConnection.ExecuteAsync(sql, new { Id = clientId }, _dbConnector.DbTransaction);
        }

        public async Task<bool> ExistsByIdAsync(string clientId)
        {
            string sql = "SELECT COUNT(1) FROM [dbo].[Client] WHERE [Id] = @Id";

            var count = await _dbConnector.DbConnection.ExecuteScalarAsync<int>(sql, new { Id = clientId }, _dbConnector.DbTransaction);

            return count > 0;
        }

        public async Task<ClientModel> GetByIdAsync(string clientId)
        {
            string sql = $"{baseSql} AND [Id] = @Id";

            var clients = await _dbConnector.DbConnection.QueryAsync<ClientModel>(sql, new { Id = clientId }, _dbConnector.DbTransaction);

            return clients.FirstOrDefault();
        }

        public async Task<List<ClientModel>> ListByFilterAsync(string clientId = null, string name = null)
        {
            string sql = baseSql;

            if (!string.IsNullOrWhiteSpace(clientId))
                sql += " AND [Id] = @Id";

            if (!string.IsNullOrWhiteSpace(name))
                sql += " AND [Name] LIKE @Name";

            var clients = await _dbConnector.DbConnection.QueryAsync<ClientModel>(
                sql,
                new { Id = clientId, Name = "%" + name + "%" },
                _dbConnector.DbTransaction);

            return clients.ToList();
        }
    }
}
