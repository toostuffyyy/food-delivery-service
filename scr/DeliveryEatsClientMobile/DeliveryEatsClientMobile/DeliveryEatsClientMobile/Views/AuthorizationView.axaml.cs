using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;

namespace DeliveryEatsClientMobile.Views;

public partial class AuthorizationView : UserControl
{
    public AuthorizationView()
    {
        InitializeComponent();
    }

    private void InputElement_OnGotFocus(object? sender, GotFocusEventArgs e)
    {
        if (sender is MaskedTextBox mtb)
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                mtb.CaretIndex = mtb.Text?.IndexOf(mtb.PromptChar) ?? 4;
            });
    }
}