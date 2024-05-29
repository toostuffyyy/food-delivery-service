namespace DeliveryEatsClientMobile.Models;

public class ParameterProduct
{
    public int ProductId { get; set; }
    public int ParameterId { get; set; }
    public string Value { get; set; } = null!;
    public Parameter Parameter { get; set; } = null!;
}