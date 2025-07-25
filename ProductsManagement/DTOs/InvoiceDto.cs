namespace ProductsManagement.DTOs
{
    public class InvoiceDto
    {
        public int InvoiceNumber { get; set; }

        public string OrderFor { get; set; }

        public string PaymentMethod { get; set; }

        public double Amount { get; set; }

        public int OrderId { get; set; }

        public List<OrderItemDto> OrderItems { get; set; }

        public DateTime IssuedAt { get; set; }

        public string IssuedBy { get; set; }
    }
}