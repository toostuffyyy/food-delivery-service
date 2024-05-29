using api.Context;
using api.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class RoleController : ControllerBase
{
    private DeliveryServiceContext _dbContext;
    public RoleController(DeliveryServiceContext context)
    {
        _dbContext = context;
    }
    
    [HttpGet("GetRole")]
    public async Task<ActionResult<IEnumerable<EmployeeRoleDTO>>> GetStatus()
    {
        try
        {
            var role = await _dbContext.EmployeeRoles.ToListAsync();
            return Ok(role.ConvertAll(a => new EmployeeRoleDTO(a)));
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status501NotImplemented);
        }
    }
}