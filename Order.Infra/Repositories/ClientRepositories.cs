using Dapper;
using Order.Domain.Interfaces.Repositories;
using Order.Domain.Interfaces.Repositories.DataConnector;
using Order.Domain.Models;

namespace Order.Infra.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly IDbConnector _dbConnector;
        public ClientRepository(IDbConnector dbConnector) => _dbConnector = dbConnector ?? throw new ArgumentNullException(nameof(dbConnector));

        private const string BaseSql = @"SELECT [Id], [Name], [Email], [PhoneNumber], [Address], [CreatedAt] FROM [dbo].[Client] WHERE 1 = 1 ";

        public async Task CreateAsync(ClientModel client)
        {
            const string sql = @"INSERT INTO [dbo].[Client] ([Id], [Name], [Email], [PhoneNumber], [Address], [CreatedAt]) VALUES (@Id, @Name, @Email, @PhoneNumber, @Address, @CreatedAt)";
            await _dbConnector.DbConnection.ExecuteAsync(sql, client, _dbConnector.DbTransaction);
        }
        public async Task UpdateAsync(ClientModel client)
        {
            const string sql = @"UPDATE [dbo].[Client] SET [Name] = @Name, [Email] = @Email, [PhoneNumber] = @PhoneNumber, [Address] = @Address WHERE [Id] = @Id";
            await _dbConnector.DbConnection.ExecuteAsync(sql, client, _dbConnector.DbTransaction);
        }
        public Task DeleteAsync(ClientModel client) => DeleteAsync(client.Id);

        public async Task DeleteAsync(string clientId)
        {
            const string sql = "DELETE FROM [dbo].[Client] WHERE [Id] = @Id";
            await _dbConnector.DbConnection.ExecuteAsync(sql, new { Id = clientId }, _dbConnector.DbTransaction);
        }
        public async Task<bool> ExistsByIdAsync(string clientId)
        {
            const string sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM [dbo].[Client] WHERE [Id] = @Id) THEN 1 ELSE 0 END";
            return await _dbConnector.DbConnection.ExecuteScalarAsync<bool>(sql, new { Id = clientId }, _dbConnector.DbTransaction);
        }
        public async Task<ClientModel> GetByIdAsync(string clientId)
        {
            return await _dbConnector.DbConnection.QueryFirstOrDefaultAsync<ClientModel>(BaseSql + " AND [Id] = @Id", new { Id = clientId }, _dbConnector.DbTransaction);
        }
        public async Task<List<ClientModel>> ListByFilterAsync(string clientId = null, string name = null)
        {
            var sql = BaseSql;
            if (!string.IsNullOrWhiteSpace(clientId)) sql += " AND [Id] = @Id";
            if (!string.IsNullOrWhiteSpace(name)) sql += " AND [Name] LIKE @Name";
            var rows = await _dbConnector.DbConnection.QueryAsync<ClientModel>(sql, new { Id = clientId, Name = $"%{name}%" }, _dbConnector.DbTransaction);
            return rows.ToList();
        }
    }
}
