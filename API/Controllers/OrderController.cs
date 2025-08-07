using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsManagement.Context;
using ProductsManagement.Data;
using Shared.Utility;
using ProductsManagement.DTOs.Mappers;
using Shared.DTOs;
using System.Security.Claims;
using QuestPDF.Fluent;
namespace ProductsManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("orders")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _dbContext.Orders
                        .Include(o => o.User)
                        .Include(o => o.Payment).Select(x => x.ToOrderSummaryDto())
                        .ToListAsync();
            return Ok(orders);
        }

        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = await _dbContext.Orders
                .Include(x => x.User)
                .Include(x => x.Payment)
                .Where(x => x.UserId == userId)
                .Select(x => x.ToOrderSummaryDto())
                .ToListAsync();
            if (orders.Count <= 0)
                return NotFound("No orders found for the current user.");

            return Ok(orders);
        }

        [HttpGet("order/{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            OrderDetailsDto? order = await _dbContext.Orders
                .Include(x => x.User)
                .Include(x => x.OrderItems)
                .ThenInclude(x => x.Product)
                .Include(x => x.Invoice)
                .Include(x => x.Payment)
                .Where(x => x.Id == id)
                .Select(x => x.ToOrderDetailsDto())
                .FirstOrDefaultAsync();
            if (order == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }
            return Ok(order);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder(CreateOrderDto dto)
        {
            try
            {
                // ✅ Group and merge order items by ProductId
                var groupedItems = dto.OrderItems
                    .GroupBy(x => x.ProductId)
                    .Select(g => new
                    {
                        ProductId = g.Key,
                        Quantity = g.Sum(x => x.Quantity)
                    }).ToList();

                double totalAmount = 0.0;
                var orderItems = new List<OrderItem>();

                foreach (var item in groupedItems)
                {
                    var product = await _dbContext.Products.FindAsync(item.ProductId);
                    if (product == null)
                    {
                        return BadRequest($"Product with ID {item.ProductId} not found.");
                    }

                    totalAmount += product.Price * item.Quantity;

                    orderItems.Add(new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price
                    });
                }

                // ✅ Create the order
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var order = new Order
                {
                    UserId = userId,
                    Status = OrderStatus.PENDING,
                    CreatedAt = DateTime.Now,
                    TotalAmount = totalAmount,
                    ShippingAddress = dto.ShippingAddress,
                    OrderItems = orderItems
                };

                _dbContext.Orders.Add(order);
                await _dbContext.SaveChangesAsync(); // Save to get the order ID

                // ✅ Create payment
                var payment = new Payment
                {
                    Method = (PaymentMethods)Enum.Parse(typeof(PaymentMethods), dto.PaymentMethod),
                    Amount = totalAmount,
                    PaidAt = DateTime.UtcNow,
                    Order = order
                };
                _dbContext.Payments.Add(payment);

                // ✅ Create invoice
                var invoice = new Invoice
                {
                    IssuedAt = DateTime.UtcNow,
                    Order = order,
                    PdfPath = Path.Combine(Directory.GetCurrentDirectory(), "Invoices", $"Invoice_{order.Id}.pdf"),
                };
                _dbContext.Invoices.Add(invoice);

                // ✅ Save all
                await _dbContext.SaveChangesAsync();

                // TO DO: Send email with invoice PDF attached

                return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order.ToOrderDetailsDto());
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while creating the order: {ex.Message}");
            }
        }

        [HttpPut("update/{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] CreateOrderDto dto)
        {
            Order? order = await _dbContext.Orders
                .Include(x => x.User)
                .Include(x => x.OrderItems)
                .ThenInclude(x => x.Product)
                .Include(x => x.Invoice)
                .Include(x => x.Payment)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }

            // Update order metadata
            order.Status = (OrderStatus)Enum.Parse(typeof(OrderStatus), dto.Status);
            order.ShippingAddress = dto.ShippingAddress;
            order.Payment.Method = (PaymentMethods)Enum.Parse(typeof(PaymentMethods), dto.PaymentMethod);

            // Deduplicate order items by ProductId and sum quantities
            var groupedItems = dto.OrderItems
                .GroupBy(x => x.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    Quantity = g.Sum(x => x.Quantity)
                }).ToList();

            // Create new OrderItems list
            order.OrderItems = groupedItems.Select(g =>
                new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = g.ProductId,
                    Quantity = g.Quantity,
                    UnitPrice = _dbContext.Products
                        .Where(p => p.Id == g.ProductId)
                        .Select(p => p.Price)
                        .FirstOrDefault() // returns 0 if not found
                }).ToList();

            _dbContext.Orders.Update(order);
            await _dbContext.SaveChangesAsync();

            return Ok(order.ToOrderSummaryDto());
        }


        [HttpDelete("destroy/{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            Order? order = _dbContext.Orders.FirstOrDefault(x => x.Id == id);

            if (order == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }
            _dbContext.Remove<Order>(order);

            await _dbContext.SaveChangesAsync();
            return NoContent();
        }



        [HttpGet("invoice/{orderId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DownloadInvoice(int orderId)
        {
            var order = await _dbContext.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return NotFound("Order not found.");

            var user = await _dbContext.Users.FindAsync(order.UserId);
            if (user == null)
                return NotFound("User not found.");

            var printableItems = order.OrderItems
                .Where(item => item.Product != null)
                .Select(item => new PrintableOrderItem
                {
                    ProductName = item.Product.Name,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price
                })
                .ToList();

            var document = new PDFGenerator(
                customerName: user.UserName,
                orderId: order.Id.ToString(),
                orderItems: printableItems,
                shippingAddress: order.ShippingAddress
            );

            var stream = new MemoryStream();
            document.GeneratePdf(stream);
            stream.Position = 0;

            // Save to /Invoices
            string invoicesDir = Path.Combine(Directory.GetCurrentDirectory(), "Invoices");
            if (!Directory.Exists(invoicesDir))
                Directory.CreateDirectory(invoicesDir);

            string filePath = Path.Combine(invoicesDir, $"Invoice_{orderId}.pdf");
            await System.IO.File.WriteAllBytesAsync(filePath, stream.ToArray());

            stream.Position = 0; // Reset again for return
            return File(stream, "application/pdf", $"Invoice_{orderId}.pdf");
        }
    }
}
