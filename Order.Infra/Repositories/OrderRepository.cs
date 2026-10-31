using Dapper;
using Order.Domain.Interfaces.Repositories;
using Order.Domain.Interfaces.Repositories.DataConnector;
using Order.Domain.Models;

namespace Order.Infra.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IDbConnector _dbConnector;
        public OrderRepository(IDbConnector dbConnector) => _dbConnector = dbConnector ?? throw new ArgumentNullException(nameof(dbConnector));

        private const string BaseSql = @"SELECT o.[Id], o.[CreatedAt],
                                                c.[Id], c.[Name], c.[Email], c.[PhoneNumber], c.[Address], c.[CreatedAt],
                                                u.[Id], u.[Name], u.[Login], u.[CreatedAt]
                                         FROM [dbo].[Order] o
                                         INNER JOIN [dbo].[Client] c ON c.[Id] = o.[ClientId]
                                         INNER JOIN [dbo].[User] u ON u.[Id] = o.[UserId]
                                         WHERE 1 = 1 ";

        public async Task CreateAsync(OrderModel order)
        {
            const string sql = @"INSERT INTO [dbo].[Order] ([Id], [ClientId], [UserId], [CreatedAt])
                                 VALUES (@Id, @ClientId, @UserId, @CreatedAt)";
            await _dbConnector.DbConnection.ExecuteAsync(sql, new
            {
                order.Id,
                ClientId = order.Client.Id,
                UserId = order.User.Id,
                order.CreatedAt
            }, _dbConnector.DbTransaction);

            foreach (var item in order.Items)
            {
                item.Order = order;
                await CreateItemAsync(item);
            }
        }

        public async Task CreateItemAsync(OrderItemModel item)
        {
            const string sql = @"INSERT INTO [dbo].[OrderItem]
                                 ([Id], [OrderId], [ProductId], [SellValue], [Quantity], [TotalAmount], [CreatedAt])
                                 VALUES (@Id, @OrderId, @ProductId, @SellValue, @Quantity, @TotalAmount, @CreatedAt)";
            await _dbConnector.DbConnection.ExecuteAsync(sql, new
            {
                item.Id,
                OrderId = item.Order.Id,
                ProductId = item.Product.Id,
                item.SellValue,
                item.Quantity,
                item.TotalAmount,
                item.CreatedAt
            }, _dbConnector.DbTransaction);
        }

        public async Task DeleteAsync(string orderId)
        {
            const string deleteItems = "DELETE FROM [dbo].[OrderItem] WHERE [OrderId] = @OrderId";
            const string deleteOrder = "DELETE FROM [dbo].[Order] WHERE [Id] = @OrderId";
            await _dbConnector.DbConnection.ExecuteAsync(deleteItems, new { OrderId = orderId }, _dbConnector.DbTransaction);
            await _dbConnector.DbConnection.ExecuteAsync(deleteOrder, new { OrderId = orderId }, _dbConnector.DbTransaction);
        }

        public async Task DeleteItemAsync(string itemId)
        {
            const string sql = "DELETE FROM [dbo].[OrderItem] WHERE [Id] = @Id";
            await _dbConnector.DbConnection.ExecuteAsync(sql, new { Id = itemId }, _dbConnector.DbTransaction);
        }

        public async Task<bool> ExistsByIdAsync(string orderId)
        {
            const string sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM [dbo].[Order] WHERE [Id] = @Id) THEN 1 ELSE 0 END";
            return await _dbConnector.DbConnection.ExecuteScalarAsync<bool>(sql, new { Id = orderId }, _dbConnector.DbTransaction);
        }

        public async Task<OrderModel> GetByIdAsync(string orderId)
        {
            var rows = await _dbConnector.DbConnection.QueryAsync<OrderModel, ClientModel, UserModel, OrderModel>(
                BaseSql + " AND o.[Id] = @Id",
                (order, client, user) =>
                {
                    order.Client = client;
                    order.User = user;
                    return order;
                },
                new { Id = orderId },
                _dbConnector.DbTransaction,
                splitOn: "Id,Id");
            return rows.FirstOrDefault();
        }

        public async Task<List<OrderModel>> ListByFilterAsync(string orderId = null, string clientId = null, string userId = null)
        {
            var sql = BaseSql;
            if (!string.IsNullOrWhiteSpace(orderId)) sql += " AND o.[Id] = @OrderId";
            if (!string.IsNullOrWhiteSpace(clientId)) sql += " AND o.[ClientId] = @ClientId";
            if (!string.IsNullOrWhiteSpace(userId)) sql += " AND o.[UserId] = @UserId";

            var rows = await _dbConnector.DbConnection.QueryAsync<OrderModel, ClientModel, UserModel, OrderModel>(
                sql,
                (order, client, user) =>
                {
                    order.Client = client;
                    order.User = user;
                    return order;
                },
                new { OrderId = orderId, ClientId = clientId, UserId = userId },
                _dbConnector.DbTransaction,
                splitOn: "Id,Id");
            return rows.ToList();
        }

        public async Task<List<OrderItemModel>> ListItemByOrderIdAsync(string orderId)
        {
            const string sql = @"SELECT oi.[Id], oi.[SellValue], oi.[Quantity], oi.[TotalAmount], oi.[CreatedAt],
                                        oi.[OrderId] AS [Id], oi.[ProductId] AS [Id],
                                        p.[Description], p.[SellValue], p.[Stock], p.[CreatedAt]
                                 FROM [dbo].[OrderItem] oi
                                 INNER JOIN [dbo].[Product] p ON p.[Id] = oi.[ProductId]
                                 WHERE oi.[OrderId] = @OrderId";
            var rows = await _dbConnector.DbConnection.QueryAsync<OrderItemModel, OrderModel, ProductModel, OrderItemModel>(
                sql,
                (item, order, product) =>
                {
                    item.Order = order;
                    item.Product = product;
                    return item;
                },
                new { OrderId = orderId },
                _dbConnector.DbTransaction,
                splitOn: "Id,Id");
            return rows.ToList();
        }

        public async Task UpdateAsync(OrderModel order)
        {
            const string updateOrder = @"UPDATE [dbo].[Order]
                                         SET [ClientId] = @ClientId, [UserId] = @UserId
                                         WHERE [Id] = @Id";
            await _dbConnector.DbConnection.ExecuteAsync(updateOrder, new
            {
                order.Id,
                ClientId = order.Client.Id,
                UserId = order.User.Id
            }, _dbConnector.DbTransaction);

            const string deleteItems = "DELETE FROM [dbo].[OrderItem] WHERE [OrderId] = @OrderId";
            await _dbConnector.DbConnection.ExecuteAsync(deleteItems, new { OrderId = order.Id }, _dbConnector.DbTransaction);

            foreach (var item in order.Items)
            {
                item.Order = order;
                await CreateItemAsync(item);
            }
        }
    }
}
