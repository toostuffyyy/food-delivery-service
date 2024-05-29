using System;
using System.Collections.Generic;

namespace api.Entities;

public partial class Employee
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public string Surname { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Patronymic { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExp { get; set; }

    public string? ImagePath { get; set; }

    public virtual EmployeeRole Role { get; set; } = null!;

    public virtual ICollection<Shift> Shifts { get; } = new List<Shift>();
}
