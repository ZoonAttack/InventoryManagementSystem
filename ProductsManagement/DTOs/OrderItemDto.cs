using ProductsManagement.Data;

namespace ProductsManagement.DTOs
{
    public class OrderItemDto
    {
        public string ProductName { get; set; }
        public string ImageURL { get; set; }

        public int Quantity { get; set; }

        public double UnitPrice { get; set; } 
    }
}