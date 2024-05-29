using api.Entities;

namespace api.DTO;

public class EmployeeRoleDTO
{
    public EmployeeRoleDTO(EmployeeRole employeeRole)
    {
        Id = employeeRole.Id;
        Name = employeeRole.Name;
    }
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}