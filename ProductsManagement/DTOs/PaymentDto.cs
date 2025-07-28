using ProductsManagement.Data.Utility;
using ProductsManagement.Data;

namespace ProductsManagement.DTOs
{
    public class PaymentDto
    {

        public string PaymentMethod { get; set; }

        public double Amount { get; set; }

        public DateTime PaidAt { get; set; }
    }
}