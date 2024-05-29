using ReactiveUI;

namespace desktop.Tools;

public class Filter : ReactiveObject
{
    private string _nameFilter;
    private bool _isPick;
    private int _value;

    public string NameFilter
    {
        get => _nameFilter;
        set => this.RaiseAndSetIfChanged(ref _nameFilter, value);
    }
    public bool IsPick
    {
        get => _isPick;
        set => this.RaiseAndSetIfChanged(ref _isPick, value);
    }
    public int Value
    {
        get => _value;
        set => this.RaiseAndSetIfChanged(ref _value, value);
    }
}