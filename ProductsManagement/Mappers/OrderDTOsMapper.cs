using ProductsManagement.Data;
using Shared.DTOs;

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
                OrderFor = order.User?.UserName ?? "Unknown User",
                OrderItems = order.OrderItems.Select(item => item.ToOrderItemDto()).ToList(),
                TotalAmount = order.TotalAmount,
                Invoice = order.Invoice?.ToInvoiceDto(order),
                Payment = order.Payment?.ToPaymentDto(order)
            };
        }
        private static PaymentDto ToPaymentDto(this Payment payment, Order order)
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
        private static InvoiceDto ToInvoiceDto(this Invoice invoice, Order order)
        {
            if (invoice == null)
                return null;
            return new InvoiceDto
            {
                InvoiceNumber = invoice.Id,
                Amount = invoice.Order.TotalAmount,
                IssuedAt = invoice.IssuedAt,
                OrderFor = order.User?.UserName ?? "Unknown User",
                OrderId = order.Id,
                OrderItems = order.OrderItems.Select(item => item.ToOrderItemDto()).ToList(),
                PaymentMethod = order.Payment?.Method.ToString() ?? "Unknown",
            };
        }
        private static OrderItemDto ToOrderItemDto(this OrderItem orderItem)
        {
            if (orderItem == null)
                return null;
            return new OrderItemDto
            {
                ProductId = orderItem.Product.Id,
                ImageURL = orderItem.Product.ImageUrl,
                Quantity = orderItem.Quantity,
            };
        }


        public static List<OrderItem> ToOrderItems(this List<OrderItemDto> orderItemDto)
        {
            if (orderItemDto == null || !orderItemDto.Any())
                return new List<OrderItem>();
            return orderItemDto.Select(item => new OrderItem
            {
                Product = new Product
                {
                    Id = item.ProductId,
                    ImageUrl = item.ImageURL
                },
                Quantity = item.Quantity,
            }).ToList();
        }
    }
}
