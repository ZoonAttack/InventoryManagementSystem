using Shared.DTOs;
using Shared.Utility;

namespace Admin.Models
{
    public class CreateOrderViewModel
    {
        public int OrderId { get; set; }
        public CreateOrderDto Order { get; set; }

        public List<ProductSummaryDto> Products { get; set; }
        public List<OrderStatus> OrderStatuses { get; set; }

        public List<PaymentMethods> PaymentMethods { get; set; }
    }
}
