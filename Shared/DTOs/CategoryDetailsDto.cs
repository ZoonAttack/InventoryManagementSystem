
namespace Shared.DTOs
{
    public class CategoryDetailsDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public List<ProductSummaryDto> Products { get; set; }
    }
}
