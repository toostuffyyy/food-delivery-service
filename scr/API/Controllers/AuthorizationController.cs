using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using api.Context;
using api.DTO;
using api.Entities;
using api.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthorizationController : ControllerBase
{
    private const int RefreshTokenExpiryTimeDay = 60;
    private DeliveryServiceContext _dbContext;
    public AuthorizationController(DeliveryServiceContext context)
    {
        _dbContext = context;
    }
    
    private string GenerateToken(IEnumerable<Claim> claims, DateTime exp)
    {
        var claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, 
            ClaimsIdentity.DefaultRoleClaimType);
        var dateNow = DateTime.UtcNow;
        var jwtToken = new JwtSecurityToken
        (
            issuer: JwtSettings.LibraryISServer,
            audience: JwtSettings.Audience,
            notBefore: dateNow,
            claims: claimsIdentity.Claims,
            expires: exp,
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSettings.Key)),
            SecurityAlgorithms.HmacSha256)
        );
        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }
    
    private string? GetIdFromJWTToken(string jwtToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var validations = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSettings.Key)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = JwtSettings.LibraryISServer,
            ValidAudience = JwtSettings.Audience,
            ValidateLifetime = true
        };
        try
        {
            var claims = handler.ValidateToken(jwtToken, validations, out var securityToken);
            return claims.FindFirstValue("id");
        }
        catch(Exception)
        {
            return null;
        }
    }
    
    [HttpGet("HashPassword")]
    public async Task<ActionResult<string>> GetHashPassword(string password)
    {
        PasswordHasher<object> passwordHasher = new PasswordHasher<object>();
        return Ok(passwordHasher.HashPassword(null, password));
    }
    
    [HttpPost("LoginEmployee")]
    public async Task<ActionResult<TokenDTO>> LoginEmployee([FromBody]AuthorizationDTO authorizationDTO)
    {
        if (string.IsNullOrEmpty(authorizationDTO.Login) || string.IsNullOrEmpty(authorizationDTO.Password))
            return BadRequest();
        try
        {
            var employee = await _dbContext.Employees
                .FirstOrDefaultAsync(a => a.Login == authorizationDTO.Login);
            if (employee == null)
                return Unauthorized();
            
            var passwordHasher = new PasswordHasher<object>();
            var result = passwordHasher.VerifyHashedPassword(null, employee.Password, authorizationDTO.Password);
            if (result == PasswordVerificationResult.Failed)
                return Unauthorized();
            
            var newDateExpiry = DateTime.Now;
            newDateExpiry = newDateExpiry.AddDays(RefreshTokenExpiryTimeDay);
            string refreshToken = RefreshTokenEmployee(employee, newDateExpiry);
            employee.RefreshToken = refreshToken;
            employee.RefreshTokenExp = newDateExpiry;
            await _dbContext.SaveChangesAsync();
            return Ok(new TokenDTO() { AccessToken = AccessTokenEmployee(employee), RefreshToken = refreshToken });
        } 
        catch (Exception)
        {
            ModelState.AddModelError("ErrorSaveChanges", "Ошибка сохранения данных");
            return BadRequest(ModelState);
        }
    }
    
    [HttpPost("UpdateTokenEmployee")]
    public async Task<ActionResult<TokenDTO>> UpdateTokenEmployee([FromHeader(Name="RefreshToken")]string refreshToken)
    {
        try
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                ModelState.AddModelError("ErrorMessage", "Неверный формат данных");
                return BadRequest(ModelState);
            }
            string? idUser = GetIdFromJWTToken(refreshToken);
            if (string.IsNullOrEmpty(idUser))
            {
                ModelState.AddModelError("ErrorMessage", "Неверный refresh token");
                return BadRequest(ModelState);
            }
            
            var employee = await _dbContext.Employees.FirstOrDefaultAsync(a => a.Id == int.Parse(idUser) && 
                                    a.RefreshToken.Equals(refreshToken));
            
            if (employee == null)
            {
                ModelState.AddModelError("ErrorMessage", "Неверный refresh token");
                return BadRequest(ModelState);
            }
            if (employee.RefreshTokenExp < DateTime.Now)
            {
                ModelState.AddModelError("ErrorMessage", "Refresh token истёк");
                return BadRequest(ModelState);
            }
            var newDateExpiry = DateTime.Now;
            newDateExpiry = newDateExpiry.AddDays(RefreshTokenExpiryTimeDay);
            string newRefreshToken = RefreshTokenEmployee(employee, newDateExpiry);
            
            employee.RefreshToken = newRefreshToken;
            employee.RefreshTokenExp = newDateExpiry;
            await _dbContext.SaveChangesAsync();
            return Ok(new TokenDTO(){AccessToken=AccessTokenEmployee(employee), RefreshToken=newRefreshToken});
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    
    [Authorize]
    [HttpPost("LogoutEmployee")]
    public async Task<ActionResult> LogoutEmployee()
    {
        try
        {
            string employeeId = User.FindFirst(ClaimTypes.Name)?.Value;
            var employee = await _dbContext.Employees
                .FirstOrDefaultAsync(a => a.Id == int.Parse(employeeId));
            if (employee == null)
                NotFound();
            employee.RefreshToken = null;
            employee.RefreshTokenExp = null;
            _dbContext.SaveChanges();
            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("CheckClient")]
    public async Task<ActionResult<bool>> CheckClient(string login)
    {
        var client = await _dbContext.Clients
            .FirstOrDefaultAsync(a => login == a.PhoneNumber || 
                                      login == a.Email);
        return client != null ? Ok(true) : Ok(false);
    }
    
    [HttpPost("LoginClient")]
    public async Task<ActionResult<TokenDTO>> LoginClient([FromBody]AuthorizationDTO authorizationDTO)
    {
        if (string.IsNullOrEmpty(authorizationDTO.Login) || string.IsNullOrEmpty(authorizationDTO.Password))
            return BadRequest();
        try
        {
            var client = await _dbContext.Clients
                .FirstOrDefaultAsync(a => authorizationDTO.Login == a.PhoneNumber || 
                                          authorizationDTO.Login == a.Email);
            if (client == null)
                return Unauthorized();
            
            var passwordHasher = new PasswordHasher<object>();
            var result = passwordHasher.VerifyHashedPassword(null, client.Password, authorizationDTO.Password);
            if (result == PasswordVerificationResult.Failed)
                return Unauthorized();
            
            var newDateExpiry = DateTime.Now;
            newDateExpiry = newDateExpiry.AddDays(RefreshTokenExpiryTimeDay);
            string refreshToken = RefreshTokenClient(client, newDateExpiry);
            client.RefreshToken = refreshToken;
            client.RefreshTokenExp = newDateExpiry;
            await _dbContext.SaveChangesAsync();
            return Ok(new TokenDTO() { AccessToken = AccessTokenClient(client), RefreshToken = refreshToken });
        } 
        catch (Exception)
        {
            ModelState.AddModelError("ErrorSaveChanges", "Ошибка сохранения данных");
            return BadRequest(ModelState);
        }
    }
    
    [HttpPost("UpdateTokenClient")]
    public async Task<ActionResult<TokenDTO>> UpdateTokenClient([FromHeader(Name="RefreshToken")]string refreshToken)
    {
        try
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                ModelState.AddModelError("ErrorMessage", "Неверный формат данных");
                return BadRequest(ModelState);
            }
            string? idUser = GetIdFromJWTToken(refreshToken);
            if (string.IsNullOrEmpty(idUser))
            {
                ModelState.AddModelError("ErrorMessage", "Неверный refresh token");
                return BadRequest(ModelState);
            }
            
            var client = await _dbContext.Clients.FirstOrDefaultAsync(a => a.Id == int.Parse(idUser) && 
                                    a.RefreshToken.Equals(refreshToken));
            
            if (client == null)
            {
                ModelState.AddModelError("ErrorMessage", "Неверный refresh token");
                return BadRequest(ModelState);
            }
            if (client.RefreshTokenExp < DateTime.Now)
            {
                ModelState.AddModelError("ErrorMessage", "Refresh token истёк");
                return BadRequest(ModelState);
            }
            var newDateExpiry = DateTime.Now;
            newDateExpiry = newDateExpiry.AddDays(RefreshTokenExpiryTimeDay);
            string newRefreshToken = RefreshTokenClient(client, newDateExpiry);
            
            client.RefreshToken = newRefreshToken;
            client.RefreshTokenExp = newDateExpiry;
            await _dbContext.SaveChangesAsync();
            return Ok(new TokenDTO(){AccessToken=AccessTokenClient(client), RefreshToken=newRefreshToken});
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    
    [Authorize]
    [HttpPost("LogoutClient")]
    public async Task<ActionResult> LogoutClient()
    {
        try
        {
            string id = User.FindFirst(ClaimTypes.Name)?.Value;
            var client = await _dbContext.Clients
                .FirstOrDefaultAsync(a => a.Id == int.Parse(id));
            if (client == null)
                NotFound();
            client.RefreshToken = null;
            client.RefreshTokenExp = null;
            _dbContext.SaveChanges();
            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    
    private string RefreshTokenEmployee(Employee employee, DateTime exp)
    {
        var claims = new List<Claim>
        {
            new Claim("id", employee.Id.ToString()),
        };
        return GenerateToken(claims, exp.ToUniversalTime());
    }
    
    private string AccessTokenEmployee(Employee employee)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, employee.Id.ToString())
        };
        return GenerateToken(claims, DateTime.UtcNow.AddMinutes(JwtSettings.LifeTime));
    }

    private string RefreshTokenClient(Client client, DateTime exp)
    {
        var claims = new List<Claim>
        {
            new Claim("id", client.Id.ToString()),
        };
        return GenerateToken(claims, exp.ToUniversalTime());
    }
    
    private string AccessTokenClient(Client client)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, client.Id.ToString())
        };
        return GenerateToken(claims, DateTime.UtcNow.AddMinutes(JwtSettings.LifeTime));
    }
}