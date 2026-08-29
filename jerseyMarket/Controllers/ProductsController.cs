using jerseyMarket.Dtos;
using jerseyMarket.Enums;
using jerseyMarket.Services;
using Microsoft.AspNetCore.Mvc;


namespace jerseyMarket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IProductService service) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<GetProductResponseDto>>> GetAllProducts([FromQuery] string? productName = null, [FromQuery] string? categoryName = null)
        {
            var Products = await service.GetAllAsync(productName, categoryName);
            return Ok(Products);
        }

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
