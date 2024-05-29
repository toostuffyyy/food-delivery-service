using System;
using System.Collections.Generic;

namespace api.Entities;

public partial class EmployeeRole
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual EmployeeSalary? EmployeeSalary { get; set; }

    public virtual ICollection<Employee> Employees { get; } = new List<Employee>();
}
