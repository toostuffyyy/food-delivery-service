using System;
using System.Collections.Generic;

namespace api.Entities;

public partial class EmployeeSalary
{
    public int EmployeeRole { get; set; }

    public decimal Salary { get; set; }

    public virtual EmployeeRole EmployeeRoleNavigation { get; set; } = null!;
}
