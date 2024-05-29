using System.Linq.Expressions;
using System.Text.Json;
using api.Context;
using api.DTO;
using api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private DeliveryServiceContext _dbContext;
    public OrderController(DeliveryServiceContext context)
    {
        _dbContext = context;
    }
    
    [Authorize]
    [HttpGet("GetOrders")]
    public async Task<ActionResult<OrderCollectionDTO>> GetOrders([FromQuery]OwnerParameters ownerParameters)
    {
        try
        {
            IQueryable<Order> ordersQuery = _dbContext.Orders
                .Include(a => a.Status)
                .Include(a => a.Review)
                .Include(a => a.Client)
                .Include(a => a.OrderStatusHistories)
                .Include(a => a.OrderItems)
                    .ThenInclude(b => b.Product)
                        .ThenInclude(c => c.CategoryProduct)
                .Include(a => a.OrderItems)
                    .ThenInclude(b => b.Product)
                        .ThenInclude(c => c.Unit)
                .Include(a => a.Pay)
                    .ThenInclude(b => b.StatusPay)
                .Include(a => a.Pay)
                    .ThenInclude(b => b.TypePay);
            
            if (!string.IsNullOrEmpty(ownerParameters.SearchString))
            {
                string search = ownerParameters.SearchString.ToLower();
                ordersQuery = ordersQuery.Where(a => a.Id.ToString().ToLower().Contains(search)
                                                     || a.Street.ToLower().Contains(search));
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
                            case "status":
                                ordersQuery = ordersQuery.Where(a => group.Select(a => a.Value)
                                    .Contains(a.StatusId));
                                break;
                            case "categoryproduct":
                                var categoryproduct = group.Select(a => a.Value).ToList();
                                ordersQuery = ordersQuery.Where(a => a.OrderItems
                                    .Any(p => categoryproduct
                                        .Contains(p.Product.CategoryProductId)));
                                break;
                            case "statuspay":
                                ordersQuery = ordersQuery.Where(a => group.Select(a => a.Value)
                                    .Contains(a.Pay.StatusPayId));
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
                    IOrderedQueryable<Order> sortOrders = null;
                    sortParameters.ForEach(x =>
                    {
                        if (sortOrders == null)
                            sortOrders = x.Direction
                                ? ordersQuery.OrderByDescending(GetSortingProperty(x.NameColumn))
                                : ordersQuery.OrderBy(GetSortingProperty(x.NameColumn));
                        else
                            sortOrders = x.Direction
                                ? sortOrders.ThenByDescending(GetSortingProperty(x.NameColumn))
                                : sortOrders.ThenBy(GetSortingProperty(x.NameColumn));
                    });
                    ordersQuery = sortOrders;
                }
            }
            int count = await ordersQuery.CountAsync();
            var orders = await ordersQuery
                .Skip((ownerParameters.PageNumber - 1) * ownerParameters.SizePage)
                .Take(ownerParameters.SizePage)
                .ToListAsync();
            return Ok(new OrderCollectionDTO(orders.ConvertAll(a => new OrderDTO(a)), count));
        }
        catch (JsonException)
        {
            return BadRequest();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    
    [Authorize]
    [HttpGet("GetOrderDetails/{id}")]
    public async Task<ActionResult<OrderDetailsDTO>> GetOrder(int id)
    {
        try
        {
            var order = await _dbContext.Orders
                .Include(a => a.OrderItems)
                    .ThenInclude(b => b.Product)
                        .ThenInclude(c => c.CategoryProduct)
                .Include(a => a.OrderItems)
                    .ThenInclude(b => b.Product)
                        .ThenInclude(c => c.Unit)
                .Include(a => a.Status)
                .Include(a => a.Client)
                .FirstOrDefaultAsync(a => a.Id == id);
            
            if (order == null)
                return NotFound();
            
            return Ok(new OrderDetailsDTO(order));
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    
    [Authorize]
    [HttpGet("GetOrdersClient/{id}")]
    public async Task<ActionResult<List<OrderDTO>>> GetOrdersClient(int id)
    {
        try
        {
            var orders = await _dbContext.Orders
                .Where(a => a.ClientId == id)
                .ToListAsync();
            
            if (orders == null)
                return NotFound();
            
            return Ok(orders.ConvertAll(a => new OrderDTO(a)));
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    
    [Authorize]
    [HttpPost("AddOrder")]
    public async Task<ActionResult> AddOrder([FromBody]OrderDetailsDTO orderDetailsDto)
    {
        try
        {
            var order = new Order()
            {
                StatusId = 1,
                MinSum = 500,
                PriceAssembly = 59,
                PriceDelivery = 109,
                ClientId = orderDetailsDto.ClientId,
                Street = orderDetailsDto.Street,
                House = orderDetailsDto.House,
                Apartment = orderDetailsDto.Apartment,
                Intercom = orderDetailsDto.Intercom,
                Floor = orderDetailsDto.Floor,
                StartDateTime = DateTime.Now,
                OrderItems = orderDetailsDto.OrderItems.Select(a => new OrderItem
                {
                    Quantity = a.Quantity,
                    ProductId = a.Product.Id
                }).ToList()
            };
            await _dbContext.Orders.AddAsync(order);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("Status code: 500", ex.InnerException.Message);
            return BadRequest(ModelState);
        }
    }

    [Authorize]
    [HttpPut("UpdateStatusOrder")]
    public async Task<ActionResult> UpdateStatusOrder([FromBody]OrderStatusDTO orderStatusDto)
    {
        try
        {
            var order = await _dbContext.Orders
                .Include(a => a.Status)
                .Include(a => a.Shifts)
                .Include(a => a.Client)
                .FirstOrDefaultAsync(a => a.Id == orderStatusDto.Id);
            if (order == null)
                return NotFound("Заказ не найден");
            order.StatusId = orderStatusDto.StatusId;
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize]
    [HttpDelete("DeleteOrder/{id}")]
    public async Task<ActionResult> DeleteOrder(int id)
    {
        try
        {
            var order = await _dbContext.Orders.FindAsync(id);
            if (order == null)
                return NotFound("Номер заказа не найден");
            _dbContext.Orders.Remove(order);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    private static Expression<Func<Order, object>> GetSortingProperty(string sortName)
    {
        return sortName?.ToLower() switch
        {
            "numberorder" => order => order.Id,
            "startdate" => order => order.StartDateTime,
            _ => order => order.Id
        };
    }
}