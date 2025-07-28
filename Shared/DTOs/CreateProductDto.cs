using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs
{
    public class CreateProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }

        public string Status { get; set; }
        public string ImageUrl { get; set; }
        public int CategoryId { get; set; }

    }
}
