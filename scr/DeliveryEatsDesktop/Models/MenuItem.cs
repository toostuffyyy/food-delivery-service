using ReactiveUI;

namespace desktop.Models;

public class MenuItem : ReactiveObject
{
    private bool _isSelected;
    public string Name { get; set; }

    public bool IsSelected
    {
        get => _isSelected;
        set => this.RaiseAndSetIfChanged(ref _isSelected, value);
    }
}