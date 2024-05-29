using api.Context;
using api.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class StatusController : ControllerBase
{
    private DeliveryServiceContext _dbContext;
    public StatusController(DeliveryServiceContext context)
    {
        _dbContext = context;
    }
    
    [HttpGet]
    [Route("GetStatus")]
    public async Task<ActionResult<IEnumerable<StatusDTO>>> GetStatus()
    {
        try
        {
            var status = await _dbContext.Statuses.ToListAsync();
            return Ok(status.ConvertAll(a => new StatusDTO(a)));
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status501NotImplemented);
        }
    }
}