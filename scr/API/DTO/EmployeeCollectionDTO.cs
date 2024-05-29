namespace api.DTO;

public class EmployeeCollectionDTO
{
    public EmployeeCollectionDTO(IEnumerable<EmployeeDTO> employees, int count)
    {
        Employees = employees;
        Count = count;
    }
    public IEnumerable<EmployeeDTO> Employees { get; set; }
    public int Count { get; set; }
}