using System.ComponentModel.DataAnnotations;

namespace jerseyMarket.Dtos
{
    // I know that is the same as CreateProductRequestDto, but I want to keep them seperated in case there a re future changes (even though it is just a task)
    public class UpdateProductRequestDto
    {
        // product must have a name
        [Required] public string ProductName { get; set; } = string.Empty;
        // price must be greater than 0
        [Required][Range(0.01, double.MaxValue)] public decimal? Price { get; set; }
        // quantity must be greater than or equal to 0
        [Required][Range(0, int.MaxValue)] public int? UnitsInStock { get; set; }
        // products must be attached to categories
        [Required] public int? CategoryId { get; set; }
    }
}
