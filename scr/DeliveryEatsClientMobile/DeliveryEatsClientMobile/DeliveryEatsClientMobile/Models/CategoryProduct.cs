namespace DeliveryEatsClientMobile.Models;

public class CategoryProduct
{
    public int Id { get; set; }
    public int? ParentCategoryProductId { get; set; }
    public string Name { get; set; } = null!;
    public string? ImagePath { get; set; }
}