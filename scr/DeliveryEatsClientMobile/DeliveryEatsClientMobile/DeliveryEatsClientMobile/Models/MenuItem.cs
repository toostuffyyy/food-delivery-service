using ReactiveUI;

namespace desktop.Models;

public class MenuItem : ReactiveObject
{
    public string Name { get; set; }
    public string Icon { get; set; }
}