using ProductsManagement.Data;

namespace ProductsManagement.DTOs
{
    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public string ImageURL { get; set; }

        public int Quantity { get; set; }
    }
}