using Shared.Utility;

namespace ProductsManagement.Data
{
    public class Payment
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public PaymentMethods Method { get; set; } // Card, COD, etc.

        public double Amount { get; set; }

        public DateTime PaidAt { get; set; }
    }
}
