using Microsoft.EntityFrameworkCore;

namespace jerseyMarket.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        [Precision(18, 2)]
        public decimal Price { get; set; }
        public int UnitsInStock { get; set; } = 0;
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

    }
}
