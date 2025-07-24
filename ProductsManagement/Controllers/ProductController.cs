using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsManagement.Context;
using ProductsManagement.Data;
using ProductsManagement.DTOs;
using ProductsManagement.DTOs.Mappers;

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

        [HttpGet("products/{category}")]
        public async Task<IActionResult> GetProductsByCategory(string category)
        {
            var products = await _dbContext.Products.Where(x => x.Category != null && x.Category.Name.Equals(category))
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

        [HttpPost("product")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
        {
            Product product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                ImageUrl = dto.ImageUrl,
                CategoryId = dto.CategoryId
            };

            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();
            return Ok(product);
        }

    }
}
