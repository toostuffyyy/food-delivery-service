using System.Collections.Generic;
using System.Threading.Tasks;
using desktop.Models;
using Refit;

namespace desktop.Service;

public interface IRoleRepository
{
    [Get("/Role/GetRole")]
    public Task<IEnumerable<EmployeeRole>> GetRole();
}