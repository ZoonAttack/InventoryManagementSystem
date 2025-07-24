using System.ComponentModel.DataAnnotations;

namespace ProductsManagement.Data
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        // Navigation
        public ICollection<Product> Products { get; set; }
    }
}
