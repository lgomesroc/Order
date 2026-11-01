using Microsoft.Extensions.Logging;
using Order.Application.DataContract.Request.Order;
using Order.Application.Interfaces;
using Order.Application.Mapper;
using Order.Domain.Interfaces.Services;
using Order.Domain.Validations.Base;
using OrderResponse = Order.Application.DataContract.Response.Order.OrderResponse;

namespace Order.Application.Applications;

public class OrderApplication(IOrderService orderService, ILogger<OrderApplication> logger) : IOrderApplication
{
    public async Task<Response> CreateAsync(CreateOrderRequest request)
    {
        try { return await orderService.CreateAsync(ApplicationMapper.ToModel(request)); }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while creating an order.");
            return Response.Unprocessable(Report.Create("The order could not be created."));
        }
    }

    public async Task<Response> UpdateAsync(UpdateOrderRequest request)
    {
        try { return await orderService.UpdateAsync(ApplicationMapper.ToModel(request)); }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while updating an order.");
            return Response.Unprocessable(Report.Create("The order could not be updated."));
        }
    }

    public Task<Response> DeleteAsync(string orderId) => orderService.DeleteAsync(orderId);

    public async Task<Response<OrderResponse>> GetByIdAsync(string orderId)
    {
        var result = await orderService.GetByIdAsync(orderId);
        return result.Report.Any()
            ? Response.Unprocessable<OrderResponse>(result.Report)
            : Response.OK(ApplicationMapper.ToResponse(result.Data));
    }

    public async Task<Response<List<OrderResponse>>> ListByFilterAsync(string orderId = null, string clientId = null, string userId = null)
    {
        var result = await orderService.ListByFiltersAsync(orderId, clientId, userId);
        return result.Report.Any()
            ? Response.Unprocessable<List<OrderResponse>>(result.Report)
            : Response.OK(result.Data.Select(ApplicationMapper.ToResponse).ToList());
    }
}
