using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsManagement.Context;
using ProductsManagement.Data;
using Shared.Utility;
using ProductsManagement.DTOs;
using ProductsManagement.DTOs.Mappers;
using Shared.DTOs;

namespace ProductsManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : Controller
    {

        private readonly ApplicationDbContext _dbContext;

        public ProductController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet("products")]
        public async Task<IActionResult> GetProducts()
        {
            List<ProductSummaryDto> products = await _dbContext.Products.Select(x => x.ToProductSummaryDto()).ToListAsync();

            return Ok(products);
        }

        [HttpGet("products/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            var products = await _dbContext.Products.Where(x => x.Category != null && x.Category.Id.Equals(categoryId))
                .Select(x => x.ToProductSummaryDto())
                .ToListAsync();

            return Ok(products);
        }

        [HttpGet("product/{productId}")]
        public IActionResult GetProduct(int productId)
        {
            var productEntity = _dbContext.Products
                .Include(x => x.Category)
                .SingleOrDefault(x => x.Id == productId);

            if (productEntity == null)
                return NotFound($"Product with ID {productId} not found.");

            var productDto = productEntity.ToProductDetailsDto();
            return Ok(productDto);
        }


        [HttpPost("create")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
        {
            Product product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                ImageUrl = dto.ImageUrl,
                CategoryId = dto.CategoryId,
                Category = await _dbContext.Categories.FindAsync(dto.CategoryId) ?? throw new ArgumentException($"Category with ID {dto.CategoryId} does not exist.")
            };
            try
            {
                _dbContext.Products.Add(product);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return BadRequest($"Error creating product: {ex.Message}");
            }
            return CreatedAtAction(nameof(GetProduct), new { productId = product.Id }, product.ToProductDetailsDto());

        }


        [HttpPut("update/{productId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateProduct(int productId, [FromBody] CreateProductDto dto)
        {
            var product = await _dbContext.Products.FindAsync(productId);
            if (product == null)
                return NotFound($"Product with ID {productId} not found.");

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Status = (ProductStatus)Enum.Parse(typeof(ProductStatus), dto.Status);
            product.ImageUrl = dto.ImageUrl;
            product.CategoryId = dto.CategoryId;
            product.Category = await _dbContext.Categories.FindAsync(dto.CategoryId)
                ?? throw new ArgumentException($"Category with ID {dto.CategoryId} does not exist.");
            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();
            return Ok(product.ToProductDetailsDto());
        }


        [HttpDelete("destroy/{productId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var product = _dbContext.Products.Find(productId);

            if (product == null)
                return NotFound($"Product with ID {productId} not found.");

            bool productHasItems = await _dbContext.OrderItems.AnyAsync(x => x.ProductId == productId);
            if(productHasItems) return BadRequest($"Product with ID {productId} cannot be deleted because it is associated with existing orders. Consider marking it as out of stock instead.");

            _dbContext.Products.Remove(product);
            _dbContext.SaveChanges();
            return Ok($"Product with ID {productId} deleted successfully.");
        }
    }
}
