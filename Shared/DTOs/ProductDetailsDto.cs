
namespace ProductsManagement.DTOs
{
    public class ProductDetailsDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public double Price { get; set; }
        public string ImageUrl { get; set; }
        public string Category { get; set; }
    }
}
