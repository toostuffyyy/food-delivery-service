using ReactiveUI;

namespace desktop.Models;

public class Sorting : ReactiveObject
{
    private bool? _direction;
    public string NameColumn { get; set; }
    public bool Direction
    {
        get => _direction.GetValueOrDefault();
        set => this.RaiseAndSetIfChanged(ref _direction, value);
    }
}