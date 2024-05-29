using api.Entities;

namespace api.DTO;

public class ParameterProductDTO
{
    public int ProductId { get; set; }
    public int ParameterId { get; set; }
    public string Value { get; set; } = null!;
    public ParameterDTO Parameter { get; set; } = null!;
    
    public ParameterProductDTO(ParameterProduct parameterProduct)
    {
        ProductId = parameterProduct.ProductId;
        ParameterId = parameterProduct.ParameterId;
        Value = parameterProduct.Value;
        Parameter = new ParameterDTO(parameterProduct.Parameter);
    }
}