using jerseyMarket.Data;
using jerseyMarket.Dtos;
using jerseyMarket.Enums;
using jerseyMarket.Models;
using Microsoft.EntityFrameworkCore;

namespace jerseyMarket.Services
{
    // I use service to handle the buisness logic outside of controller -> clean code
    public class ProductService (AppDbContext _context) : IProductService
    {
        public async Task<List<GetProductResponseDto>> GetAllAsync(string? productName = null, string? categoryName = null, CancellationToken cancellationToken = default) // add the cancellationToken parameter to the method signature - if backend is abrupted it saves resources
        {
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            // does not filter if one of the parameters is null or empty, so we can call this method with no parameters to get all products
            if (!string.IsNullOrWhiteSpace(productName))
            {
                query = query.Where(p => p.ProductName.Contains(productName));
            }

            if (!string.IsNullOrWhiteSpace(categoryName))
            {
                query = query.Where(p => p.Category.CategoryName == categoryName);
            }

            return await query
                .Select(p => new GetProductResponseDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    UnitsInStock = p.UnitsInStock,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.CategoryName
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<(SingleProductResult Result, GetProductResponseDto? Product)> GetSingleAsync(int id, CancellationToken cancellationToken) // add the cancellationToken parameter to the method signature - if backend is abrupted it saves resources
        {
            var product = await _context.Products
                .Where(p => p.ProductId == id)
                .Select(p => new GetProductResponseDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    UnitsInStock = p.UnitsInStock,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.CategoryName
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (product is null)
            {
                return (SingleProductResult.ProductNotFound, null);
            }

            return (SingleProductResult.Success, product);
        }

        public async Task<(SingleProductResult Result, GetProductResponseDto? Product)> AddAsync(CreateProductRequestDto product)
        {
            // FindAsync (not AnyAsync) so we already have the entity for CategoryName below, no second query needed
            var category = await _context.Categories.FindAsync(product.CategoryId);

            if (category is null)
            {
                return (SingleProductResult.CategoryNotFound, null);
            }

            var newProduct = new Product
            {
                ProductName = product.ProductName,
                // Safe to unwrap: [Required] on the DTO already rejected null before the action ran
                Price = product.Price!.Value,
                UnitsInStock = product.UnitsInStock!.Value,
                CategoryId = product.CategoryId!.Value,
            };

            _context.Products.Add(newProduct);

            await _context.SaveChangesAsync();

            return (SingleProductResult.Success, new GetProductResponseDto
            {
                ProductId = newProduct.ProductId,
                ProductName = newProduct.ProductName,
                Price = newProduct.Price,
                UnitsInStock = newProduct.UnitsInStock,
                CategoryId = newProduct.CategoryId,
                CategoryName = category.CategoryName
            });
        }

        public async Task<(SingleProductResult Result, GetProductResponseDto? Product)> UpdateAsync(int id, UpdateProductRequestDto Product)
        {
            var currentProduct = await _context.Products.FindAsync(id);

            // if the product is not found, we return ProductNotFound and null for the product
            if (currentProduct is null)
            {
                return (SingleProductResult.ProductNotFound, null);
            }

            var category = await _context.Categories.FindAsync(Product.CategoryId);

            // if category is not found, we return CategoryNotFound and null for the product
            if (category is null)
            {
                return (SingleProductResult.CategoryNotFound, null);
            }

            currentProduct.ProductName = Product.ProductName;
            // Safe to unwrap: [Required] on the DTO already rejected null before the action ran
            currentProduct.Price = Product.Price!.Value;
            currentProduct.UnitsInStock = Product.UnitsInStock!.Value;
            currentProduct.CategoryId = Product.CategoryId!.Value;

            await _context.SaveChangesAsync();

            return (SingleProductResult.Success, new GetProductResponseDto
            {
                ProductId = currentProduct.ProductId,
                ProductName = currentProduct.ProductName,
                Price = currentProduct.Price,
                UnitsInStock = currentProduct.UnitsInStock,
                CategoryId = currentProduct.CategoryId,
                CategoryName = category.CategoryName
            });
        }
    }
}
