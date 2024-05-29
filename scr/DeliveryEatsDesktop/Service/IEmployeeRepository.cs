using System.Threading.Tasks;
using desktop.Models;
using Refit;

namespace desktop.Service;

public interface IEmployeeRepository
{
    [Get("/Employee/GetInfo")]
    public Task<Employee> GetInfo([Authorize("Bearer")] string accessToken);
    
    [Get("/Employee/GetEmployees")]
    public Task<EmployeeCollection> GetEmployees([Authorize("Bearer")] string accessToken, [Query]OwnerParametersEmployee ownerParameters);
    
    [Get("/Employee/GetEmployeeEdit/{id}")]
    public Task<EmployeeEdit> GetEmployeeEdit([Authorize("Bearer")] string accessToken, [AliasAs("id")]int id);
    
    [Put("/Employee/UpdateEmployee")]
    public Task UpdateEmployee([Authorize("Bearer")] string accessToken, [Body]EmployeeEdit employeeEdit);
    
    [Post("/Employee/AddEmployee")]
    public Task AddEmployee([Authorize("Bearer")] string accessToken, [Body]EmployeeEdit employeeEdit);

    [Delete("/Employee/DeleteEmployee/{id}")]
    public Task DeleteEmployee([Authorize("Bearer")] string accessToken, [AliasAs("id")] int id);
}