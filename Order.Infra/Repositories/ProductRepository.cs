using Dapper;
using Order.Domain.Interfaces.Repositories;
using Order.Domain.Interfaces.Repositories.DataConnector;
using Order.Domain.Models;

namespace Order.Infra.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IDbConnector _dbConnector;

        public ProductRepository(IDbConnector dbConnector)
        {
            _dbConnector = dbConnector;
        }

        const string baseSql = @"SELECT [Id]
                                       ,[Description]
                                       ,[SellValue]
                                       ,[Stock]
                                       ,[CreatedAt]
                                   FROM [dbo].[Product]
                                  WHERE 1 = 1 ";

        public async Task CreateAsync(ProductModel product)
        {
            string sql = @"INSERT INTO [dbo].[Product]
                                  ([Id], [Description], [SellValue], [Stock], [CreatedAt])
                           VALUES (@Id, @Description, @SellValue, @Stock, @CreatedAt)";

            await _dbConnector.DbConnection.ExecuteAsync(sql, new
            {
                product.Id,
                product.Description,
                product.SellValue,
                product.Stock,
                product.CreatedAt
            }, _dbConnector.DbTransaction);
        }

        public async Task DeleteAsync(string productId)
        {
            string sql = "DELETE FROM [dbo].[Product] WHERE [Id] = @Id";

            await _dbConnector.DbConnection.ExecuteAsync(sql, new { Id = productId }, _dbConnector.DbTransaction);
        }

        public async Task<bool> ExistsByIdAsync(string productId)
        {
            string sql = "SELECT COUNT(1) FROM [dbo].[Product] WHERE [Id] = @Id";

            var count = await _dbConnector.DbConnection.ExecuteScalarAsync<int>(sql, new { Id = productId }, _dbConnector.DbTransaction);

            return count > 0;
        }

        public async Task<ProductModel> GetByIdAsync(string productId)
        {
            string sql = $"{baseSql} AND [Id] = @Id";

            var products = await _dbConnector.DbConnection.QueryAsync<ProductModel>(sql, new { Id = productId }, _dbConnector.DbTransaction);

            return products.FirstOrDefault();
        }

        public async Task<List<ProductModel>> ListByFilterAsync(string productId = null, string description = null)
        {
            string sql = baseSql;

            if (!string.IsNullOrWhiteSpace(productId))
                sql += " AND [Id] = @Id";

            if (!string.IsNullOrWhiteSpace(description))
                sql += " AND [Description] LIKE @Description";

            var products = await _dbConnector.DbConnection.QueryAsync<ProductModel>(
                sql,
                new { Id = productId, Description = "%" + description + "%" },
                _dbConnector.DbTransaction);

            return products.ToList();
        }

        public async Task UpdateAsync(ProductModel product)
        {
            string sql = @"UPDATE [dbo].[Product]
                              SET [Description] = @Description
                                 ,[SellValue] = @SellValue
                                 ,[Stock] = @Stock
                            WHERE [Id] = @Id";

            await _dbConnector.DbConnection.ExecuteAsync(sql, new
            {
                product.Id,
                product.Description,
                product.SellValue,
                product.Stock
            }, _dbConnector.DbTransaction);
        }
    }
}
