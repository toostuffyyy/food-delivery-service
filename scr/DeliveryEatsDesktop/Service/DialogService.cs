using System.Threading.Tasks;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace desktop.Service;

public class DialogService : IDialogService
{
    public async Task<IDialogService.DialogResult> ShowDialog(string title, string description, IDialogService.DialogType dialogType)
    {
        var box = dialogType == IDialogService.DialogType.Standart
            ? MessageBoxManager.GetMessageBoxStandard(title, description)
            : MessageBoxManager.GetMessageBoxStandard(title, description, ButtonEnum.YesNo);
        return (IDialogService.DialogResult)await box.ShowAsync();
    }
}