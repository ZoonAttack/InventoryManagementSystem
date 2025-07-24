using System.ComponentModel.DataAnnotations;

namespace ProductsManagement.Data
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public bool IsAvailable { get; set; }

        public string ImageUrl { get; set; } 

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        // Navigation
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
