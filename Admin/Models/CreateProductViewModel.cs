using Shared.DTOs;
using Shared.Utility;

namespace Admin.Models
{
    public class CreateProductViewModel
    {
        public int ProductId { get; set; }
        public CreateProductDto Product { get; set; }

        public List<CategorySummaryDto> Categories { get; set; }

        public List<ProductStatus> productStatuses { get; set; }
    }
}
