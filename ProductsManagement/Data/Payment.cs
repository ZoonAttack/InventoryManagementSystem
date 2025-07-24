using ProductsManagement.Data.Utility;

namespace ProductsManagement.Data
{
    public class Payment
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public PaymentMethods Method { get; set; } // Card, COD, etc.

        public decimal Amount { get; set; }

        public DateTime PaidAt { get; set; }
    }
}
