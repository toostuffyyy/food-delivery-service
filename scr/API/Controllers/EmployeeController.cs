using System.Linq.Expressions;
using System.Security.Claims;
using System.Text.Json;
using api.Context;
using api.DTO;
using api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class EmployeeController : ControllerBase
{
    private DeliveryServiceContext _dbContext;
    public EmployeeController(DeliveryServiceContext context)
    {
        _dbContext = context;
    }
    
    [Authorize]
    [HttpGet("GetInfo")]
    public async Task<ActionResult<EmployeeDTO>> GetInfo()
    {
        string employeeId = User.FindFirst(ClaimTypes.Name)?.Value;
        var employee = await _dbContext.Employees
            .Include(a => a.Role)
            .FirstOrDefaultAsync(a => a.Id == int.Parse(employeeId));
        if (employee == null)
            NotFound();
        return Ok(new EmployeeDTO(employee));
    }
    
    [Authorize]
    [HttpGet("GetEmployees")]
    public async Task<ActionResult<EmployeeCollectionDTO>> GetManagers([FromQuery]OwnerParameters ownerParameters)
    {
        try
        {
            IQueryable<Employee> employeeQuery = _dbContext.Employees
                .Include(a => a.Role);
            if (!string.IsNullOrEmpty(ownerParameters.SearchString))
            {
                string search = ownerParameters.SearchString.ToLower();
                employeeQuery = employeeQuery.Where(a => a.Surname.ToLower().Contains(search) || 
                                                       a.Name.ToLower().Contains(search) ||
                                                     a.Role.Name.ToLower().Contains(search));
            }
            if (!string.IsNullOrEmpty(ownerParameters.Filters))
            {
                List<FilterParameter>? filter = JsonSerializer.Deserialize<List<FilterParameter>>(ownerParameters.Filters);
                if (filter != null && filter.Count > 0)
                {
                    var groupFilters = filter.GroupBy(a => a.NameParameter);
                    foreach (var group in groupFilters)
                    {
                        switch (group.Key.ToLower())
                        {
                            case "role":
                                employeeQuery = employeeQuery.Where(a => group.Select(a => a.Value).Contains(a.RoleId));
                                break;
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(ownerParameters.Sorts))
            {
                List<SortingDTO>? sortParameters = JsonSerializer.Deserialize<List<SortingDTO>>(ownerParameters.Sorts);
                if (sortParameters != null && sortParameters.Count > 0)
                {
                    IOrderedQueryable<Employee> sortOrders = null;
                    sortParameters.ForEach(x =>
                    {
                        if (sortOrders == null)
                            sortOrders = x.Direction
                                ? employeeQuery.OrderByDescending(GetSortingProperty(x.NameColumn))
                                : employeeQuery.OrderBy(GetSortingProperty(x.NameColumn));
                        else
                            sortOrders = x.Direction
                                ? sortOrders.ThenByDescending(GetSortingProperty(x.NameColumn))
                                : sortOrders.ThenBy(GetSortingProperty(x.NameColumn));
                    });
                    employeeQuery = sortOrders;
                }
            }
            int count = await employeeQuery.CountAsync();
            var employees = await employeeQuery.Skip((ownerParameters.PageNumber - 1) * ownerParameters.SizePage)
                .Take(ownerParameters.SizePage)
                .ToListAsync();
            return Ok(new EmployeeCollectionDTO(employees.ConvertAll(a => new EmployeeDTO(a)), count));
        }
        catch (JsonException)
        {
            return BadRequest();
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
        
    [Authorize]
    [HttpGet]
    [Route("GetEmployeeEdit/{id}")]
    public async Task<ActionResult<EmployeeEditDTO>> GetManagerEdit(int id)
    {
        try
        {
            var employee = await _dbContext.Employees
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (employee == null)
                return NotFound("Сотрудник не найден");
            return Ok(new EmployeeEditDTO(employee));
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    
    [Authorize]
    [HttpPost]
    [Route("AddEmployee")]
    public async Task<ActionResult> AddManager([FromBody] EmployeeEditDTO employeeEditDto)
    {
        try
        {
            var passwordHasher = new PasswordHasher<object>();
            Employee employee = new Employee()
            {
                RoleId = employeeEditDto.RoleId,
                Surname = employeeEditDto.Surname,
                Name = employeeEditDto.Name,
                Patronymic = employeeEditDto.Patronymic,
                Email = employeeEditDto.Email,
                PhoneNumber = employeeEditDto.PhoneNumber,
                Login = employeeEditDto.Login,
                Password = passwordHasher.HashPassword(null, employeeEditDto.Password),
                ImagePath = employeeEditDto.ImagePath
            };
            await _dbContext.Employees.AddAsync(employee);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status501NotImplemented);
        }
    }
    
    [Authorize]
    [HttpPut]
    [Route("UpdateEmployee")]
    public async Task<ActionResult<EmployeeEditDTO>> UpdateManager([FromBody]EmployeeEditDTO employeeEditDto)
    {
        try
        {
            var passwordHasher = new PasswordHasher<object>();
            var employee = await _dbContext.Employees
                .Include(a => a.Role)
                    .ThenInclude(b => b.EmployeeSalary)
                .FirstOrDefaultAsync(a => a.Id == employeeEditDto.Id);
            if (employee == null)
                return NotFound("Менеджер не найден");
            employee.RoleId = employeeEditDto.RoleId;
            employee.Surname = employeeEditDto.Surname;
            employee.Name = employeeEditDto.Name;
            employee.Patronymic = employeeEditDto.Patronymic;
            employee.Email = employeeEditDto.Email;
            employee.PhoneNumber = employeeEditDto.PhoneNumber;
            employee.Login = employeeEditDto.Login;
            employee.Password = passwordHasher.HashPassword(null, employeeEditDto.Password);
            employee.ImagePath = employeeEditDto.ImagePath;
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    
    [Authorize]
    [HttpDelete]
    [Route("DeleteEmployee/{id}")]
    public async Task<ActionResult> DeleteManager(int id)
    {
        try
        {
            var employee = await _dbContext.Employees.FindAsync(id);
            if (employee == null)
                return NotFound("Менеджер не найден");
            _dbContext.Employees.Remove(employee);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    
    private static Expression<Func<Employee, object>> GetSortingProperty(string sortName)
    {
        return sortName?.ToLower() switch
        {
            "surname" => employee => employee.Surname,
            "name" => employee => employee.Name,
            "role" => employee => employee.Role.Name,
            _ => employee => employee.Id
        };
    }
}