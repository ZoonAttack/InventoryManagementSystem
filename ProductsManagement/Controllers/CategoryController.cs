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
            var categories = await _dbContext.Categories.Select(x =>
            new CategoryDto()
            {
                Name = x.Name,
                Description = x.Description
            }).ToListAsync();

            return Ok(categories);
        }


        [HttpPost("create")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDto categoryDto)
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
            return CreatedAtAction(nameof(GetCategories), new { id = category.Id }, category);
        }
    }
}
