using jerseyMarket.Dtos;
using jerseyMarket.Enums;

namespace jerseyMarket.Services
{
    // use interface to define the contract for the ProductService, allowing for easier testing and flexibility in implementation
    public interface IProductService
    {
        Task<List<GetProductResponseDto>> GetAllAsync(string? productName = null, string? categoryName = null, CancellationToken cancellationToken = default); // add the cancellationToken parameter to the method signature - if backend is abrupted it saves resources
        Task<(SingleProductResult Result, GetProductResponseDto? Product)> AddAsync(CreateProductRequestDto Product);
        Task<(SingleProductResult Result, GetProductResponseDto? Product)> UpdateAsync(int id, UpdateProductRequestDto Product);
    }
}
