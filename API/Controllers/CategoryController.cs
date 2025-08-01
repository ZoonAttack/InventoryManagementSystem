using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsManagement.Context;
using ProductsManagement.Data;
using Shared.DTOs;

namespace ProductsManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public CategoryController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _dbContext.Categories
                                            .Include(c => c.Products)
                                            .Select(x => new CategorySummaryDto() {
                                                    CategoryId = x.Id,
                                                    Name = x.Name,
                                                    Description = x.Description
                                            }).ToListAsync();

            return Ok(categories);
        }


        [HttpGet("category/{name}")]
        public async Task<IActionResult> GetCategory(string name)
        {
            var category = await _dbContext.Categories
                                           .Include(c => c.Products)
                                           .Where(c => c.Name == name)
                                           .Select(x => new CategoryDetailsDto()
                                           {
                                               CategoryId = x.Id,
                                               Name = x.Name,
                                               Description = x.Description,
                                               Products = x.Products.Select(p => new ProductSummaryDto
                                               {
                                                   Name = p.Name,
                                                   Status = p.Status.ToString(),
                                                   Price = p.Price,
                                                   ImageUrl = p.ImageUrl
                                               }).ToList()
                                           }).FirstOrDefaultAsync();
            if (category == null)
            {
                return NotFound("Category not found.");
            }
            return Ok(category);
        }

        [HttpPost("create")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto categoryDto)
        {
            if (categoryDto == null || string.IsNullOrEmpty(categoryDto.Name))
            {
                return BadRequest("Invalid category data.");
            }
            var category = new Category
            {
                Name = categoryDto.Name,
                Description = categoryDto.Description
            };
            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCategory), new { name = category.Name }, category);
        }
    }
}
