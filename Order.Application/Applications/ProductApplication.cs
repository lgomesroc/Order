using AutoMapper;
using Order.Application.DataContract.Request.Product;
using Order.Application.DataContract.Response.Product;
using Order.Application.Interfaces;
using Order.Domain.Interfaces.Services;
using Order.Domain.Models;
using Order.Domain.Validations.Base;
namespace Order.Application.Applications
{
    public class ProductApplication : IProductApplication
    {
        private readonly IProductService _service;
        private readonly IMapper _mapper;
        public ProductApplication(IProductService service, IMapper mapper) { _service = service; _mapper = mapper; }
        public async Task<Response> CreateAsync(CreateProductRequest request) => await _service.CreateAsync(_mapper.Map<ProductModel>(request));
        public async Task<Response<ProductResponse>> GetByIdAsync(string id)
        {
            var result = await _service.GetByIdAsync(id);
            return result.Report.Any() ? Response.Unprocessable<ProductResponse>(result.Report) : Response.OK(_mapper.Map<ProductResponse>(result.Data));
        }
        public async Task<Response<List<ProductResponse>>> ListByFilterAsync(string id = null, string description = null)
        {
            var result = await _service.ListByFilterAsync(id, description);
            return result.Report.Any() ? Response.Unprocessable<List<ProductResponse>>(result.Report) : Response.OK(_mapper.Map<List<ProductResponse>>(result.Data));
        }
        public Task<Response> UpdateAsync(UpdateProductRequest request) => _service.UpdateAsync(_mapper.Map<ProductModel>(request));
        public Task<Response> DeleteAsync(string id) => _service.DeleteAsync(id);
    }
}
