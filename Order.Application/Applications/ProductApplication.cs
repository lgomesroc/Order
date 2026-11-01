using Microsoft.Extensions.Logging;
using Order.Application.DataContract.Request.Product;
using Order.Application.DataContract.Response.Product;
using Order.Application.Interfaces;
using Order.Application.Mapper;
using Order.Domain.Interfaces.Services;
using Order.Domain.Validations.Base;

namespace Order.Application.Applications;

public class ProductApplication(IProductService productService, ILogger<ProductApplication> logger) : IProductApplication
{
    public async Task<Response> CreateAsync(CreateProductRequest request)
    {
        try { return await productService.CreateAsync(ApplicationMapper.ToModel(request)); }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while creating a product.");
            return Response.Unprocessable(Report.Create("The product could not be created."));
        }
    }

    public async Task<Response<ProductResponse>> GetByIdAsync(string productId)
    {
        var result = await productService.GetByIdAsync(productId);
        return result.Report.Any()
            ? Response.Unprocessable<ProductResponse>(result.Report)
            : Response.OK(ApplicationMapper.ToResponse(result.Data));
    }

    public async Task<Response<List<ProductResponse>>> ListByFilterAsync(string productId = null, string description = null)
    {
        var result = await productService.ListByFilterAsync(productId, description);
        return result.Report.Any()
            ? Response.Unprocessable<List<ProductResponse>>(result.Report)
            : Response.OK(result.Data.Select(ApplicationMapper.ToResponse).ToList());
    }

    public Task<Response> UpdateAsync(UpdateProductRequest request) =>
        productService.UpdateAsync(ApplicationMapper.ToModel(request));

    public Task<Response> DeleteAsync(string productId) => productService.DeleteAsync(productId);
}
