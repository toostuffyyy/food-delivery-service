using api.Entities;

namespace api.DTO;

public class StatusPayDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    
    public StatusPayDTO(StatusPay statusPay)
    {
        Id = statusPay.Id;
        Name = statusPay.Name;
    }
}