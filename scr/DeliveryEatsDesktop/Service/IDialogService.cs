using System.Threading.Tasks;

namespace desktop.Service;

public interface IDialogService
{
    public enum DialogType
    {
        Standart,
        YesNoDialog
    }
    public enum DialogResult
    {
        Ok,
        Yes,
        No
    }
    public Task<DialogResult> ShowDialog(string title, string description, DialogType dialogType);
}