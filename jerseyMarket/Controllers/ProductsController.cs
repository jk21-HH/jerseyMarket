using jerseyMarket.Dtos;
using jerseyMarket.Enums;
using jerseyMarket.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace jerseyMarket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IProductService service) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<GetProductResponseDto>>> GetAllProducts([FromQuery] string? productName = null, [FromQuery] string? categoryName = null, CancellationToken cancellationToken = default) // add the cancellationToken parameter to the method signature - if backend is abrupted it saves resources
        {
            var Products = await service.GetAllAsync(productName, categoryName, cancellationToken);
            return Ok(Products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetProductResponseDto>> GetProduct(int id, CancellationToken cancellationToken) // add the cancellationToken parameter to the method signature - if backend is abrupted it saves resources
        {
            var (result, Product) = await service.GetSingleAsync(id, cancellationToken);

            return result switch
            {
                SingleProductResult.Success => Ok(Product),
                SingleProductResult.ProductNotFound => NotFound($"Product with ID {id} not found."),
                _ => StatusCode(500) // unreachable unless the enum grows and a case is missed
            };
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<GetProductResponseDto>> CreateProduct(CreateProductRequestDto Product)
        {
            var (result, createdProduct) = await service.AddAsync(Product);

            return result switch
            {
                SingleProductResult.Success => Ok(createdProduct),
                SingleProductResult.CategoryNotFound => NotFound($"Category with ID {Product.CategoryId} not found."),
                _ => StatusCode(500) // unreachable unless the enum grows and a case is missed
            };
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<GetProductResponseDto>> UpdateProduct(int id, UpdateProductRequestDto Product)
        {
            var (result, updatedProduct) = await service.UpdateAsync(id, Product);

            return result switch
            {
                SingleProductResult.Success => Ok(updatedProduct),
                SingleProductResult.ProductNotFound => NotFound($"Product with ID {id} not found."),
                SingleProductResult.CategoryNotFound => NotFound($"Category with ID {Product.CategoryId} not found."),
                _ => StatusCode(500) // unreachable unless the enum grows and a case is missed
            };
        }
    }
}
