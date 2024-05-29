using api.Entities;

namespace api.DTO;

public class StatusDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    public StatusDTO(Status status)
    {
        Id = status.Id;
        Name = status.Name;
    }
}