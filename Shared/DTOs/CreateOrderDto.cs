namespace Shared.DTOs
{
    public class CreateOrderDto
    {
        //public string Note { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }

        public string ShippingAddress { get; set; }
        public string PaymentMethod { get; set; }

        public string Status { get; set; }
    }
}
