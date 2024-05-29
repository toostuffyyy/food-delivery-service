using api.Context;
using api.DTO;
using api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class CategoryProductController : ControllerBase
{
    private DeliveryServiceContext _dbContext;
    public CategoryProductController(DeliveryServiceContext context)
    {
        _dbContext = context;
    }
    
    [HttpGet("GetCategoryProduct")]
    public async Task<ActionResult<IEnumerable<CategoryProductDTO>>> GetCategoryProduct()
    {
        try
        {
            var categoryProducts = await _dbContext.CategoryProducts.ToListAsync();
            return Ok(categoryProducts.ConvertAll(a => new CategoryProductDTO(a)));
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status501NotImplemented);
        }
    }
    
    [HttpGet("GetCategoryProduct/{id}")]
    public async Task<ActionResult<CategoryProductDTO>> GetCategoryProduct(int id)
    {
        try
        {
            var categoryProducts = await _dbContext.CategoryProducts
                .FindAsync(id);
            return Ok(new CategoryProductDTO(categoryProducts));
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status501NotImplemented);
        }
    }
    
    [HttpGet("GetCategoryProductMainPage")]
    public async Task<ActionResult<IEnumerable<CategoryProductDTO>>> GetCategoryProductMainPage()
    {
        try
        {
            var status = await _dbContext.CategoryProducts
                .Take(7)
                .ToListAsync();
            
            status.Add(new CategoryProduct() {Name = "Категории"});
            
            return Ok(status.ConvertAll(a => new CategoryProductDTO(a)));
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status501NotImplemented);
        }
    }
}