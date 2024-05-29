using desktop.Models;
using ReactiveUI;

namespace desktop.Tools;

public class SortElement : ReactiveObject
{
    private bool _isSelected;
    public Sorting Sorting { get; set; }
    public string NameSort { get; set; }
    public bool IsSelected
    {
        get => _isSelected;
        set => this.RaiseAndSetIfChanged(ref _isSelected, value);
    }
}