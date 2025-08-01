using Shared.Utility;

namespace ProductsManagement.Data
{
    public class Order
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public OrderStatus Status { get; set; }

        // One User
        public string UserId { get; set; }
        public User User { get; set; }

        // Many OrderItems
        public ICollection<OrderItem> OrderItems { get; set; }

        public double TotalAmount { get; set; }

        // Foreign key to Invoice
        public int? InvoiceId { get; set; }
        public Invoice Invoice { get; set; }

        // One Payment
        public int? PaymentId { get; set; }
        public Payment Payment { get; set; }
        public string ShippingAddress { get; set; }
    }

}
