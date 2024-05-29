using api.Context;
using api.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class StatusPayController : ControllerBase
{
    private DeliveryServiceContext _dbContext;
    public StatusPayController(DeliveryServiceContext context)
    {
        _dbContext = context;
    }
    
    [HttpGet]
    [Route("GetStatusPay")]
    public async Task<ActionResult<IEnumerable<StatusPayDTO>>> GetStatusPay()
    {
        try
        {
            var status = await _dbContext.StatusPays.ToListAsync();
            return Ok(status.ConvertAll(a => new StatusPayDTO(a)));
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status501NotImplemented);
        }
    }
}