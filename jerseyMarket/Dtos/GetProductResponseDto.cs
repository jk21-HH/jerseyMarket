namespace jerseyMarket.Dtos
{
    public class GetProductResponseDto
    {
        // data transfer object for returning product information to the client
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int UnitsInStock { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
