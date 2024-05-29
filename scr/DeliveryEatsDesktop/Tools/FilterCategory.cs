using System.Collections.ObjectModel;
using ReactiveUI;

namespace desktop.Tools;

public class FilterCategory : ReactiveObject
{
    private string _nameCategory;
    private ObservableCollection<Filter> _filters;

    public string ParameterName { get; set; }
    public string NameCategory
    {
        get => _nameCategory;
        set => this.RaiseAndSetIfChanged(ref _nameCategory, value);
    }

    public ObservableCollection<Filter> Filters
    {
        get => _filters;
        set => this.RaiseAndSetIfChanged(ref _filters, value);
    }
}