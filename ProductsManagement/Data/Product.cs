using ProductsManagement.Data.Utility;
using System.ComponentModel.DataAnnotations;

namespace ProductsManagement.Data
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public ProductStatus Status { get; set; }

        public double Price { get; set; }
        public string ImageUrl { get; set; } 

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        // Navigation
        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}
