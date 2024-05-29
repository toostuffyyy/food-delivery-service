using api.Entities;

namespace api.DTO;

public class PromotionDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public int Discount { get; set; }
    public string? Description { get; set; }
    
    public PromotionDTO(Promotion promotion)
    {
        Id = promotion.Id;
        Name = promotion.Name;
        StartDateTime = promotion.StartDateTime;
        EndDateTime = promotion.EndDateTime;
        Discount = promotion.Discount;
        Description = promotion.Description;
    }
}