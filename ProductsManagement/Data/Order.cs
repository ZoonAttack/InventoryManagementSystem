using ProductsManagement.Data.Utility;

namespace ProductsManagement.Data
{
    public class Order
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public OrderStatus Status { get; set; } 

        public int UserId { get; set; }
        public User User { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }

        public decimal TotalAmount { get; set; } 
    }
}
