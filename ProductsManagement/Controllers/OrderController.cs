using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsManagement.Context;
using ProductsManagement.Data;
using ProductsManagement.DTOs;
using ProductsManagement.DTOs.Mappers;
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

        [HttpPost("create")]


        [HttpGet("orders")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _dbContext.Orders.Select(x => x.ToOrderSummaryDto()).ToListAsync();
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
