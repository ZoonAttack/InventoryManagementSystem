namespace Shared.DTOs
{
    public class OrderDetailsDto
    {
        public int OrderId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public string OrderFor { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
        public double TotalAmount { get; set; }

        public string ShippingAddress { get; set; }

        public InvoiceDto Invoice { get; set; }
        public PaymentDto Payment { get; set; }
    }
}
