using ProductsManagement.Data.Utility;
using ProductsManagement.Data;

namespace ProductsManagement.DTOs
{
    public class PaymentDto
    {
        public int PaymentID { get; set; }

        public int OrderId { get; set; }

        //public string UserId { get; set; }

        public string PaymentMethod { get; set; }

        public double Amount { get; set; }

        public DateTime PaidAt { get; set; }
    }
}