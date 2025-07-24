using System.ComponentModel.DataAnnotations;

namespace ProductsManagement.Data
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        // Many products
        public ICollection<Product> Products { get; set; }
    }
}
