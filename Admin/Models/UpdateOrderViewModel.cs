using Shared.DTOs;
using Shared.Utility;

namespace Admin.Models
{
    public class UpdateOrderViewModel
    {
        public int OrderId { get; set; }
        public CreateOrderDto Order { get; set; }

        public List<OrderStatus> OrderStatuses { get; set; }
    }
}
