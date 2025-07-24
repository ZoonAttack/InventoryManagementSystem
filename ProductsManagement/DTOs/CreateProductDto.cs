using System.ComponentModel.DataAnnotations;

namespace ProductsManagement.DTOs
{
    public class CreateProductDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public double Price { get; set; }

        public string ImageUrl { get; set; }
        [Required]
        public int CategoryId { get; set; }

    }
}
