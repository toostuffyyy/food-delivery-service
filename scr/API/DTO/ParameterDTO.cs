using Parameter = api.Entities.Parameter;

namespace api.DTO;

public class ParameterDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    
    public ParameterDTO(Parameter parameter)
    {
        Id = parameter.Id;
        Name = parameter.Name;
    }
}