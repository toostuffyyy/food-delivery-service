namespace desktop.Models;

public class Employee
{
    public int Id { get; set; }
    public string Role { get; set; }
    public string Surname { get; set; }
    public string Name { get; set; }
    public string? Patronymic { get; set; }
    public string? ImagePath { get; set; }
}