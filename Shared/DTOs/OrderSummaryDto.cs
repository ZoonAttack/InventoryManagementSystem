

namespace Shared.DTOs
{
    public class OrderSummaryDto
    {
        public DateTime CreatedAt { get; set; }

        public string Status { get; set; }

        public string OrderFor { get; set; }

        public double TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
    }
}
