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
            // SELECT Products.*, Categories.*
            // FROM Products
            // INNER JOIN Categories ON Products.CategoryId = Categories.CategoryId
            // WHERE Products.ProductName LIKE '%' + @productName + '%'
            // AND Categories.CategoryName LIKE '%' + @categoryName+ '%'

            var query = _context.Products.Include(p => p.Category).AsQueryable();

            // does not filter if one of the parameters is null or empty, so we can call this method with no parameters to get all products
            if (!string.IsNullOrWhiteSpace(productName))
            {
                query = query.Where(p => p.ProductName.Contains(productName));
            }

            if (!string.IsNullOrWhiteSpace(categoryName))
            {
                query = query.Where(p => p.Category.CategoryName.Contains(categoryName));
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
            // SELECT TOP(1) Products.ProductId, Products.ProductName, Products.Price, Products.UnitsInStock, Products.CategoryId, Categories.CategoryName
            // FROM Products INNER JOIN Categories ON Products.CategoryId = Categories.CategoryId
            // WHERE Products.ProductId = @id

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
            // SELECT CASE WHEN EXISTS (SELECT 1 FROM Products WHERE ProductName = @product_ProductName) THEN 1 ELSE 0 END

            if (await _context.Products.AnyAsync(p => p.ProductName == product.ProductName))
            {
                return (SingleProductResult.ProductNameTaken, null);
            }

            // SELECT TOP(1) *
            // FROM Categories
            // WHERE CategoryId = @product_CategoryId

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

            // INSERT INTO Products(ProductName, Price, UnitsInStock, CategoryId)
            // VALUES(@ProductName, @Price, @UnitsInStock, @CategoryId)

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
            // SELECT TOP(1) *
            // FROM Categories
            // WHERE CategoryId = @product_CategoryId

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

            // UPDATE Products
            // SET ProductName = @p0, Price = @p1, UnitsInStock = @p2, CategoryId = @p3
            // WHERE ProductId = @p4

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
