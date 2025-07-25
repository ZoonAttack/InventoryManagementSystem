using ProductsManagement.Data;

namespace ProductsManagement.DTOs.Mappers
{
    public static class OrderDTOsMapper
    {
        public static OrderSummaryDto ToOrderSummaryDto(this Order order)
        {
            if (order == null)
                return null;
            return new OrderSummaryDto
            {
                CreatedAt = order.CreatedAt,
                Status = order.Status.ToString(),
                OrderFor = order.User?.UserName ?? "Unknown User",
                TotalAmount = order.TotalAmount,
                PaymentMethod = order.Payment.Method.ToString() ?? "Unknown"
            };
        }

        public static OrderDetailsDto ToOrderDetailsDto(this Order order)
        {
            if (order == null)
                return null;
            return new OrderDetailsDto
            {
                OrderId = order.Id,
                CreatedAt = order.CreatedAt,
                Status = order.Status.ToString(),
                UserName = order.User?.UserName ?? "Unknown User",
                OrderItems = order.OrderItems.Select(item => item.ToOrderItemDto()).ToList(),
                TotalAmount = order.TotalAmount,
                Invoice = order.Invoice?.ToInvoiceDto(),
                Payment = order.Payment?.ToPaymentDto()
            };
        }
        private static PaymentDto ToPaymentDto(this Payment payment)
        {
            if (payment == null)
                return null;
            return new PaymentDto
            {
                PaymentMethod = payment.Method.ToString(),
                Amount = payment.Amount,
                PaidAt = payment.PaidAt
            };
        }
        private static InvoiceDto ToInvoiceDto(this Invoice invoice)
        {
            if (invoice == null)
                return null;
            return new InvoiceDto
            {
                InvoiceNumber = invoice.Id,
                Amount = invoice.Order.TotalAmount,
                IssuedAt = invoice.IssuedAt
            };
        }
        private static OrderItemDto ToOrderItemDto(this OrderItem orderItem)
        {
            if (orderItem == null)
                return null;
            return new OrderItemDto
            {
                ProductName = orderItem.Product.Name,
                ImageURL = orderItem.Product.ImageUrl,
                Quantity = orderItem.Quantity,
                UnitPrice = orderItem.UnitPrice
            };
        }
    }
}
