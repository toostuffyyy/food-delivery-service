using System.Security.Claims;
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
public class ClientController : ControllerBase
{
    private DeliveryServiceContext _dbContext;
    public ClientController(DeliveryServiceContext context)
    {
        _dbContext = context;
    }
    
    [Authorize]
    [HttpGet("GetInfo")]
    public async Task<ActionResult<ClientDTO>> GetInfo()
    {
        string Id = User.FindFirst(ClaimTypes.Name)?.Value;
        var client = await _dbContext.Clients
            .Include(a => a.Addresses)
            .FirstOrDefaultAsync(a => a.Id == int.Parse(Id));
        return client != null ? Ok(new ClientDTO(client)) : NotFound("Клиент не найден");
    }
        
    [Authorize]
    [HttpGet("GetClientDetails/{id}")]
    public async Task<ActionResult<ClientDetailsDTO>> GetClientDetails(int id)
    {
        try
        {
            var client = await _dbContext.Clients
                .Include(a => a.Addresses)
                .FirstOrDefaultAsync(a => a.Id == id);
            return client != null ? Ok(new ClientDetailsDTO(client)) : NotFound("Клиент не найден");
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    
    [HttpPost("AddClient")]
    public async Task<ActionResult> AddClient([FromBody] ClientDetailsDTO clientDetailsDto)
    {
        try
        {
            var passwordHasher = new PasswordHasher<object>();
            Client client = new()
            {
                Name = clientDetailsDto.Name,
                PhoneNumber = clientDetailsDto?.PhoneNumber,
                Email = clientDetailsDto?.Email,
                Password = passwordHasher.HashPassword(null, clientDetailsDto.Password),
                ImagePath = clientDetailsDto?.ImagePath,
                Addresses = clientDetailsDto.Addresses.Select(a => new Address()
                {
                   Street = a.Street,
                   House = a.House,
                   Apartment = a.Apartment,
                   Intercom = a?.Intercom,
                   Floor = a?.Floor,
                   Comment = a?.Comment
                }).ToList()
            };
            await _dbContext.Clients.AddAsync(client);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status501NotImplemented);
        }
    }
    
    //[Authorize]
    [HttpPut("UpdateClient")]
    public async Task<ActionResult<ClientDetailsDTO>> UpdateClient([FromBody]ClientDetailsDTO clientDetailsDto)
    {
        try
        {
            var client = await _dbContext.Clients
                .Include(a => a.Addresses)
                .FirstOrDefaultAsync(a => a.Id == clientDetailsDto.Id);
            if (client == null)
                return NotFound("Клиент не найден");
            client.Name = clientDetailsDto.Name;
            client.PhoneNumber = clientDetailsDto.PhoneNumber;
            client.Email = clientDetailsDto.Email;
            client.Password = clientDetailsDto.Password;
            client.ImagePath = clientDetailsDto.ImagePath;
            client.Addresses = clientDetailsDto.Addresses.Select(a => new Address()
            {
                Street = a.Street,
                House = a.House,
                Apartment = a.Apartment,
                Intercom = a.Intercom,
                Floor = a.Floor,
                Comment = a.Comment
            }).ToList();
            
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    
    [Authorize]
    [HttpDelete("DeleteClient/{id}")]
    public async Task<ActionResult> DeleteClient(int id)
    {
        try
        {
            var client = await _dbContext.Clients
                .FirstOrDefaultAsync(a => a.Id == id);
            if (client == null)
                return NotFound("Клиент не найден");
            
            _dbContext.Clients.Remove(client);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}