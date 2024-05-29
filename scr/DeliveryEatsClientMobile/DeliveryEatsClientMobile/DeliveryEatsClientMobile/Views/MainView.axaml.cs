using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Input;
using DeliveryEatsClientMobile.Service;
using Splat;

namespace DeliveryEatsClientMobile.Views;

public partial class MainView : UserControl
{
    private bool _isPressed = false;
    public MainView()
    {
        InitializeComponent();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        Locator.Current.GetService<INotificationService>().RegisterNotificationManager(new WindowNotificationManager(TopLevel.GetTopLevel(this)));
    }

    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _isPressed = true;
    }
}