using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsManagement.Context;
using ProductsManagement.Data;
using Shared.Utility;
using ProductsManagement.DTOs.Mappers;
using Shared.DTOs;
using System.Security.Claims;

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
                .Where(x => x.UserId == userId)
                .Select(x => x.ToOrderSummaryDto())
                .ToListAsync();
            if(orders.Count <= 0)
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
            if(order == null)
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
                //1- Calculate total amount
                var totalAmount = 0.0;
                foreach (var item in dto.OrderItems)
                {
                    Product? product = await _dbContext.Products.FindAsync(item.ProductId);
                    if (product == null)
                    {
                        return BadRequest($"Product with ID {item.ProductId} not found.");
                    }
                    totalAmount += product.Price * item.Quantity;
                    item.ProductId = product.Id;
                    item.ImageURL = product.ImageUrl;
                }
                //2- create the order
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                Order order = new Order()
                {
                    UserId = userId,
                    Status = OrderStatus.PENDING,
                    CreatedAt = DateTime.UtcNow,
                    TotalAmount = totalAmount,
                    ShippingAddress = dto.ShippingAddress,
                    OrderItems = dto.OrderItems.Select(item => new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = _dbContext.Products.SingleOrDefault(x => x.Id == item.ProductId)?.Price ?? 0
                    }).ToList()
                };
                _dbContext.Orders.Add(order);
                await _dbContext.SaveChangesAsync(); // Save to get the order ID for the next steps
                //3- create the payment(cash does not need api)
                Payment payment = new()
                {
                    Method = PaymentMethods.COD,
                    Amount = totalAmount,
                    PaidAt = DateTime.UtcNow,
                    Order = order
                };
                _dbContext.Payments.Add(payment);
                //4- create the invoice
                Invoice invoice = new()
                {
                    IssuedAt = DateTime.UtcNow,
                    Order = order,
                    PdfPath = "LATER"
                };
                _dbContext.Invoices.Add(invoice);
                //5 - save order, payment, and invoice in database
                await _dbContext.SaveChangesAsync();
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
            order.Status = (OrderStatus)Enum.Parse(typeof(OrderStatus), dto.Status);
            order.ShippingAddress = dto.ShippingAddress;
            order.Payment.Method = (PaymentMethods)Enum.Parse(typeof(PaymentMethods),dto.PaymentMethod);
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
    }
}
