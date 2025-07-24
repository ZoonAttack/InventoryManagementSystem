using ProductsManagement.Data.Utility;

namespace ProductsManagement.Data
{
    public class Order
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public OrderStatus Status { get; set; } 

        //One user
        public string UserId { get; set; }
        public User User { get; set; }

        //Many
        public ICollection<OrderItem> OrderItems { get; set; }

        public double TotalAmount { get; set; }
        //One invoice
        public Invoice Invoice { get; set; }
        //One Payment
        public Payment Payment { get; set; }
    }
}
