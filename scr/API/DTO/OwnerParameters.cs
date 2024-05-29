namespace api.DTO;

public class OwnerParameters
{
    public const int MaxSizePage = 35;
    private int _sizePage = 15;
    public int PageNumber { get; set; } = 1;
    public int SizePage
    {
        get => _sizePage;
        set => _sizePage = value > MaxSizePage ? MaxSizePage : value;
    }
    public string? SearchString { get; set; }
    public string? Filters { get; set; }
    public string? Sorts { get; set; }
}