using Dapper;
using Order.Domain.Interfaces.Repositories;
using Order.Domain.Interfaces.Repositories.DataConnector;
using Order.Domain.Models;

namespace Order.Infra.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnector _dbConnector;

        public UserRepository(IDbConnector dbConnector)
        {
            _dbConnector = dbConnector;
        }

        // [User] is a reserved word in SQL Server, so the brackets are mandatory.
        const string baseSql = @"SELECT [Id]
                                       ,[Name]
                                       ,[Login]
                                       ,[PasswordHash]
                                       ,[CreatedAt]
                                   FROM [dbo].[User]
                                  WHERE 1 = 1 ";

        public async Task CreateAsync(UserModel user)
        {
            string sql = @"INSERT INTO [dbo].[User]
                                  ([Id], [Name], [Login], [PasswordHash], [CreatedAt])
                           VALUES (@Id, @Name, @Login, @PasswordHash, @CreatedAt)";

            await _dbConnector.DbConnection.ExecuteAsync(sql, new
            {
                user.Id,
                user.Name,
                user.Login,
                user.PasswordHash,
                user.CreatedAt
            }, _dbConnector.DbTransaction);
        }

        public async Task UpdateAsync(UserModel user)
        {
            string sql = @"UPDATE [dbo].[User]
                              SET [Name] = @Name
                                 ,[Login] = @Login
                                 ,[PasswordHash] = @PasswordHash
                            WHERE [Id] = @Id";

            await _dbConnector.DbConnection.ExecuteAsync(sql, new
            {
                user.Id,
                user.Name,
                user.Login,
                user.PasswordHash
            }, _dbConnector.DbTransaction);
        }

        public async Task DeleteAsync(string userId)
        {
            string sql = "DELETE FROM [dbo].[User] WHERE [Id] = @Id";

            await _dbConnector.DbConnection.ExecuteAsync(sql, new { Id = userId }, _dbConnector.DbTransaction);
        }

        public async Task<bool> ExistsByIdAsync(string userId)
        {
            string sql = "SELECT COUNT(1) FROM [dbo].[User] WHERE [Id] = @Id";

            var count = await _dbConnector.DbConnection.ExecuteScalarAsync<int>(sql, new { Id = userId }, _dbConnector.DbTransaction);

            return count > 0;
        }

        public async Task<bool> ExistsByLoginAsync(string login)
        {
            string sql = "SELECT COUNT(1) FROM [dbo].[User] WHERE [Login] = @Login";

            var count = await _dbConnector.DbConnection.ExecuteScalarAsync<int>(sql, new { Login = login }, _dbConnector.DbTransaction);

            return count > 0;
        }

        public async Task<UserModel> GetByIdAsync(string userId)
        {
            string sql = $"{baseSql} AND [Id] = @Id";

            var users = await _dbConnector.DbConnection.QueryAsync<UserModel>(sql, new { Id = userId }, _dbConnector.DbTransaction);

            return users.FirstOrDefault();
        }

        public async Task<UserModel> GetByLoginAsync(string login)
        {
            string sql = $"{baseSql} AND [Login] = @Login";

            var users = await _dbConnector.DbConnection.QueryAsync<UserModel>(sql, new { Login = login }, _dbConnector.DbTransaction);

            return users.FirstOrDefault();
        }

        public async Task<List<UserModel>> ListByFilterAsync(string userId = null, string name = null)
        {
            string sql = baseSql;

            if (!string.IsNullOrWhiteSpace(userId))
                sql += " AND [Id] = @Id";

            if (!string.IsNullOrWhiteSpace(name))
                sql += " AND [Name] LIKE @Name";

            var users = await _dbConnector.DbConnection.QueryAsync<UserModel>(
                sql,
                new { Id = userId, Name = "%" + name + "%" },
                _dbConnector.DbTransaction);

            return users.ToList();
        }
    }
}
