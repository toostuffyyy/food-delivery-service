using Avalonia.Controls;
using desktop.Services;
using Splat;

namespace desktop.Views;

public partial class EmployeeWindow : Window
{
    public EmployeeWindow()
    {
        InitializeComponent();
        Locator.Current.GetService<IFilePickerService>().RegisterProvider(TopLevel.GetTopLevel(this));
    }
}