using api.Context;
using api.DTO;
using api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("[controller]")]
public class StatisticController : ControllerBase
{
    private readonly DeliveryServiceContext _dbContext;
    public StatisticController(DeliveryServiceContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [Authorize]
    [HttpGet]
    [Route("GetProductStatistics")]
    public async Task<ActionResult<IEnumerable<ProductStatisticDTO>>> GetOrderStatistics([FromQuery] DateRangeDTO dateRange)
    {
        IQueryable<OrderItem> query = _dbContext.OrderItems
            .Include(a => a.Product)
                .ThenInclude(b => b.CategoryProduct)
            .Include(a => a.Product)
                .ThenInclude(b => b.Unit);
        
        if(dateRange.StartDate != null && dateRange.EndDate != null && dateRange.StartDate <= dateRange.EndDate)
            query = query.Where(x=>x.Order.StartDateTime >= dateRange.StartDate && x.Order.EndDateTime <= dateRange.EndDate);
        try
        {
            var result = await query.GroupBy(a => a.Product).ToListAsync();
            return Ok(result.ConvertAll(p => 
                new ProductStatisticDTO(new ProductDTO(p.Key), p.Sum(item => item.Quantity), p.Sum(a => a.Product.Price * a.Quantity))));
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
