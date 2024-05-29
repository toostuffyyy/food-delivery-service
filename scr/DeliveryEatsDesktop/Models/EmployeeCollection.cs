using System.Collections;
using System.Collections.Generic;

namespace desktop.Models;

public class EmployeeCollection
{
    public IEnumerable<Employee> Employees { get; set; }
    public int Count { get; set; }
}