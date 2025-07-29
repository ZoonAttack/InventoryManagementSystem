using Shared.DTOs;

namespace Admin.Models
{
    public class AdminDashboardViewModel
    {
        public List<ProductSummaryDto> Products { get; set; }

        public List<OrderSummaryDto> Orders { get; set; }
    }
}
